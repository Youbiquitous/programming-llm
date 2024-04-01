"""
Description: Contains functions for creating a retrieval chain for language models.
"""

from operator import itemgetter
from langchain.retrievers.multi_query import MultiQueryRetriever
from langchain_core.retrievers import BaseRetriever
from langchain_core.output_parsers import StrOutputParser
from langchain_core.messages import get_buffer_string
from langchain_core.runnables import (
    RunnableBranch,
    RunnableLambda,
    RunnableParallel,
    RunnablePassthrough,
)
from .prompts import CONDENSE_QUESTION_PROMPT, ANSWER_PROMPT
from .formatting import _combine_documents, _format_chat_history

def create_retrieval_chain(llm, llm_condenser, llm_reworder, retriever: BaseRetriever):
    """
    Create a retrieval chain for language models.

    Args:
    - llm (AzureChatOpenAI): The main language model.
    - llm_condenser (AzureChatOpenAI): The language model for condensing user input.
    - llm_reworder (AzureChatOpenAI): The language model for rewording user input.
    - retriever (BaseRetriever): An instance of a retriever implementing the BaseRetriever interface.

    Returns:
    - RunnableBranch: The retrieval chain.
    """
    enhanced_retriever = MultiQueryRetriever.from_llm(retriever=retriever, llm=llm_reworder)

    search_query = RunnableBranch(
        (
            RunnableLambda(lambda x: bool(x["chat_history"])),
            RunnablePassthrough.assign(
                chat_buffer=lambda x: get_buffer_string(_format_chat_history(x["chat_history"]))
            )
            | CONDENSE_QUESTION_PROMPT
            | llm_condenser
            | StrOutputParser(),
        ),
        RunnableLambda(itemgetter("question"))
    )

    answer_chain = (
        ANSWER_PROMPT
        | llm
        | StrOutputParser()
    )

    rag_chain = (RunnableParallel(
        {
            "docs": search_query | enhanced_retriever,
            "question": lambda x: x["question"],
            "chat_history": lambda x: _format_chat_history(x["chat_history"])
        }
    )
    .assign(context=lambda x: _combine_documents(x["docs"]))
    .assign(answer=answer_chain)
    .pick(["answer", "docs"])
    )
    
    return rag_chain