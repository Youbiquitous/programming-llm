"""
Description: Contains utility functions for combining documents and formatting chat history.
"""
from typing import List
#from xml.dom.minidom import Document
from langchain_core.messages import AIMessage, HumanMessage, BaseMessage
from langchain_core.documents import Document
from langchain_core.prompts import format_document
from .prompts import DEFAULT_COMBINE_DOCUMENT_PROMPT

def _combine_documents(docs: List[Document], document_separator="\n\n") -> str:
    """
    Combine a list of documents into a formatted string.

    Args:
    - docs (list): List of documents to be combined.
    - document_separator (str): Separator between documents (default: "\n\n").

    Returns:
    - str: Combined and formatted document string.
    """
    """Convert Documents to a single string.:"""
    doc_strings = [format_document(doc, DEFAULT_COMBINE_DOCUMENT_PROMPT) for doc in docs]
    return document_separator.join(doc_strings)

def _format_chat_history(chat_history=[]) -> List[BaseMessage]:
    """
    Format a chat history into a list of BaseMessage objects.

    Args:
    - chat_history (list): List of messages in the chat history.

    Returns:
    - list[BaseMessage]: List of BaseMessage objects representing the chat history.
    """
    msg_buffer = []
    for message in chat_history:
        if message["role"]=="assistant":
            msg_buffer.append(AIMessage(content=message["content"]))
        else:
            msg_buffer.append(HumanMessage(content=message["content"]))
    return msg_buffer
