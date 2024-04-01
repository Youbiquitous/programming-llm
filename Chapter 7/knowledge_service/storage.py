"""
Contains a class to handle the storage of documents and embeddings.
"""

import os
import uuid
from typing import List, Dict, Optional
from langchain.output_parsers.openai_tools import PydanticToolsParser
from langchain.storage import LocalFileStore
from langchain.storage._lc_store import create_kv_docstore
from langchain.text_splitter import RecursiveCharacterTextSplitter
from langchain.retrievers import MultiVectorRetriever
from langchain.retrievers.multi_vector import SearchType
from langchain_core.documents import Document
from langchain_core.retrievers import BaseRetriever
from langchain_core.output_parsers import StrOutputParser
from langchain_core.utils.function_calling import convert_to_openai_tool
from langchain_openai import AzureChatOpenAI
from langchain_openai.embeddings import AzureOpenAIEmbeddings
from langchain_community.document_transformers.openai_functions import create_metadata_tagger
from langchain_community.document_loaders.csv_loader import CSVLoader
from langchain_community.document_loaders.directory import DirectoryLoader
from langchain_community.document_loaders.pdf import PyPDFLoader
from langchain_community.document_loaders.xml import UnstructuredXMLLoader
from langchain_community.vectorstores.chroma import Chroma

from chromadb.config import Settings

from .prompting.ingestion import FAQ_PROMPT, SUMMARY_PROMPT
from .tools.faq import hypothetical_questions
from .tools.tag import Properties


# Define a dictionary to map file extensions to respective loaders
_loaders = {
    '.pdf': PyPDFLoader,
    '.xml': UnstructuredXMLLoader,
    '.csv': CSVLoader,
}

def _create_directory_loader(file_type, directory_path) -> DirectoryLoader:
    if file_type not in _loaders:
        raise ValueError(f"File type '{file_type}' not supported!")
    return DirectoryLoader(
        path=directory_path,
        glob=f"**/*{file_type}",
        loader_cls=_loaders[file_type],
        use_multithreading=False
    )

class StorageService:
    """A class to handle the storage of documents and embeddings."""

    def __init__(self, deployment: str, embedding_deployment: str, fallbacks: Optional[List[str]] = None, verbosity=False, temperature=0, root_data_folder = "./"):
        """
        Initialize the StorageService.

        Args:
        - deployment (str): Azure deployment for language models.
        - embedding_deployment (str): Azure deployment for embeddings.
        """
        # Create fallback models if provided
        fallback_llms = [AzureChatOpenAI(azure_deployment=fallback, temperature=temperature, verbose=verbosity) for fallback in (fallbacks or [])]

        self.llm = AzureChatOpenAI(max_retries=0, azure_deployment=deployment).with_fallbacks(fallback_llms)
        self.embedding_model = AzureOpenAIEmbeddings(azure_deployment=embedding_deployment)
        
        # file store for original full doc
        docstore_folder = os.path.join(root_data_folder, 'documents')
        self.docstore = create_kv_docstore(LocalFileStore(docstore_folder))

        # usual vector store for chunks
        chroma_db_folder = os.path.join(root_data_folder, 'chroma_db')
        self.vectorstore = Chroma(
            embedding_function=self.embedding_model,
            persist_directory=chroma_db_folder,
            client_settings= Settings(anonymized_telemetry=False, is_persistent=True)
        )
        self.parent_id_key = 'doc_id'

    def load_pdf_files(self, folder_path:str='data') :
        """
        Load a single file into storage.

        Args:
        - folder_path (str): Path to the folder to be loaded.
        """
        # create one DirectoryLoader for each file type
        pdf_loader = _create_directory_loader('.pdf', folder_path)
        pdf_documents = pdf_loader.load()
        self._load_in_storage(pdf_documents)

    def _load_in_storage(self, documents: List[Document]):
        """
        Load a Document object into storage.

        Args:
        - document (Document): Document object to be loaded.
        """

        # preparing the 'smaller chunks' strategy
        parent_splitter = RecursiveCharacterTextSplitter(
            chunk_size=2000, chunk_overlap=400)
            
        parent_docs = parent_splitter.split_documents(documents)
        child_splitter = RecursiveCharacterTextSplitter(chunk_size=300)

        # preparing the FAQ generation strategy
        parser = PydanticToolsParser(tools=[hypothetical_questions])
        faq_chain = (
            {"doc": lambda x: x.page_content}
            | FAQ_PROMPT
            | self.llm.bind(tools=[convert_to_openai_tool(hypothetical_questions)], 
                            tool_choice={"type": "function", "function": {"name": "hypothetical_questions"}})
            | parser
        )

        # preparing the summary strategy
        summary_chain = (
            {"doc": lambda x: x.page_content}
            | SUMMARY_PROMPT
            | self.llm
            | StrOutputParser()
        )

        # preparing the meta tagging strategy, if any
        document_transformer = create_metadata_tagger(Properties, self.llm)
        parent_docs = document_transformer.transform_documents(parent_docs)

        # create document ids and add docs to the docstore
        parent_doc_ids = [str(uuid.uuid4()) for _ in parent_docs]
        self.docstore.mset(list(zip(parent_doc_ids, parent_docs)))

        # doing work on the parent docs
        smaller_chunks = []
        question_docs = []
        summary_docs = []


        # generating and adding FAQs
        faqs = faq_chain.batch(parent_docs, {"max_concurrency": 5})
        for i, question_list in enumerate(faqs):
            # metadata to use with FAQs and summaries
            sub_strategies_metadata = parent_docs[i].metadata
            sub_strategies_metadata[self.parent_id_key] = parent_doc_ids[i]
            question_docs.extend([Document(page_content=s, metadata=sub_strategies_metadata) 
                                    for s in question_list[0].questions])
        
        # generating and adding summaries
        summaries = summary_chain.batch(parent_docs, {"max_concurrency": 5})
        
        for i, summary in enumerate(summaries):
            # metadata to use with FAQs and summaries
            sub_strategies_metadata = parent_docs[i].metadata
            sub_strategies_metadata[self.parent_id_key] = parent_doc_ids[i]
            summary_docs.extend([Document(page_content=summary, metadata=sub_strategies_metadata)])
        
        for i, doc in enumerate(parent_docs):
            _id = parent_doc_ids[i]

            # small chunks
            _sub_docs = child_splitter.split_documents([doc])
            for _doc in _sub_docs:
                # standard metadata are already copied by splitting
                _doc.metadata[self.parent_id_key] = _id
            smaller_chunks.extend(_sub_docs)

        if smaller_chunks and len(smaller_chunks) > 0:
            self.vectorstore.add_documents(smaller_chunks)
        if question_docs and len(question_docs) > 0:
            self.vectorstore.add_documents(question_docs)
        if summary_docs and len(summary_docs) > 0:
            self.vectorstore.add_documents(summary_docs)

    def get_retriever(self) -> BaseRetriever:
        """
        Get a retriever.

        Returns:
        - BaseRetriever: Retriever.
        """
        retriever = MultiVectorRetriever(
            vectorstore=self.vectorstore,
            docstore=self.docstore,
            id_key=self.parent_id_key,
            search_type=SearchType.mmr
        )
        return retriever




    def get_loaded_files(self) -> set[str]:
        """
        Get a set of loaded files.

        Returns:
        - set[str]: Set of loaded files.
        """
        loaded_files = set()
        result = self.vectorstore.get(include=['metadatas'])
        for metadata in result['metadatas']:
            if metadata.get('source'):
                loaded_files.add(metadata['source'])
        return loaded_files
    
    def remove_loaded_file(self, file_path: str, batch_size=5461):
        """
        Remove a specific file from the vector store (batchsize needed for Chroma DB)

        Returns:
        - set[str]: Set of loaded files.
        """
        items = self.vectorstore.get(where = {'source': file_path}, include=['metadatas'])

        # collect used parent ids
        parent_ids = set()
        for metadata in items['metadatas']:
            if metadata.get(self.parent_id_key):
                parent_ids.add(metadata[self.parent_id_key])
        # delete docs
        if parent_ids and len(parent_ids) > 0:
            print(parent_ids)
            self.docstore.mdelete(list(parent_ids))

        # needed for Chroma DB
        batches = [items['ids'][i:i+batch_size] for i in range(0, len(items), batch_size)]
        for batch in batches:
            # delete vectors
            self.vectorstore.delete(ids=batch)
