"""
Module: user_chat.py
Description: Contains the UserChat class for managing interactions with language models.
"""
from typing import List, Dict, Optional
from langchain_openai import AzureChatOpenAI
from langchain_core.retrievers import BaseRetriever
from .chain import create_retrieval_chain

class ChatService:
    """
    Class for managing user interactions with language models.

    Parameters:
    - llm_deployment (str): Deployment configuration for the main language model.
    - llm_condenser_deployment (str): Deployment configuration for the condenser language model.
    - llm_reworder_deployment (str): Deployment configuration for the reworder language model.
    - retriever (BaseRetriever): An instance of a retriever implementing the BaseRetriever interface.
    - verbosity (bool, optional): Verbosity flag for detailed output. Defaults to False.
    - temperature (int, optional): Temperature parameter for language models. Defaults to 0.

    Attributes:
    - llm_condenser (AzureChatOpenAI): Instance of AzureChatOpenAI for condensing user input.
    - llm_reworder (AzureChatOpenAI): Instance of AzureChatOpenAI for rewording user input.
    - llm (AzureChatOpenAI): Instance of AzureChatOpenAI for main language processing.
    - retriever (BaseRetriever): The retriever instance.
    - chain: The conversational retrieval chain.
    """

    def __init__(self, llm_deployment: str, llm_condenser_deployment: str, llm_reworder_deployment: str, retriever: BaseRetriever, fallbacks: Optional[List[str]] = None, verbosity=True, temperature=0):
        """
        Initialize UserChat with language models and retriever.

        Args:
        - llm_deployment (str): Deployment configuration for the main language model.
        - llm_condenser_deployment (str): Deployment configuration for the condenser language model.
        - llm_reworder_deployment (str): Deployment configuration for the reworder language model.
        - retriever (BaseRetriever): An instance of a retriever implementing the BaseRetriever interface.
        - fallbacks (list, optional): List of deployment configurations for fallback language models. Defaults to None.        
        - verbosity (bool, optional): Verbosity flag for detailed output. Defaults to False.
        - temperature (int, optional): Temperature parameter for language models. Defaults to 0.
        """
        # Create fallback models if provided
        fallback_llms = [AzureChatOpenAI(azure_deployment=fallback, temperature=temperature, verbose=verbosity, streaming=True) for fallback in (fallbacks or [])]

        # Different model for different purposes
        llm_condenser = AzureChatOpenAI(azure_deployment=llm_condenser_deployment, temperature=temperature, streaming=True).with_fallbacks(fallback_llms)
        llm_reworder = AzureChatOpenAI(azure_deployment=llm_reworder_deployment, temperature=temperature, streaming=True).with_fallbacks(fallback_llms)

        llm = AzureChatOpenAI(azure_deployment=llm_deployment, temperature=temperature, verbose=verbosity, streaming=True).with_fallbacks(fallback_llms)

        self.retriever = retriever
        self.chain = create_retrieval_chain(llm, llm_condenser, llm_reworder, retriever)

    def run_rag(self, query: str, chat_history:Optional[List[Dict[str, str]]] =[]):
        """
        Run the RAG (Retrieval-Augmented Generation) process.

        Args:
        - query (str): The user's query.
        - chat_history (list of dict, optional): List of previous chat messages.
          Each message is represented as a dictionary with 'role' and 'content' keys.
          Defaults to None.

        Returns:
        - dict: The result of the RAG process.
        """
        return self.chain.invoke({"question": query, "chat_history": chat_history,})