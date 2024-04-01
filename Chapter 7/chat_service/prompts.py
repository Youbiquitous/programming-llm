from langchain_core.prompts.chat import ChatPromptTemplate, MessagesPlaceholder
from langchain.prompts.prompt import PromptTemplate

# Define prompts and templates
_condense_question_template = """Given the following conversation and a follow up question, 
rephrase the follow up question to be a standalone question, to retrieve relevant documents from a vector  database.
Chat History:
{chat_buffer}
Follow Up Input: {question}
Standalone question:"""
CONDENSE_QUESTION_PROMPT = PromptTemplate.from_template(_condense_question_template)

_answer_template = """You are an enterprise AI assistant.
Answer the question based only on the following context, IF IT IS HELPFUL:
<context>
{context}
</context>
If you don't know the answer, just say that you don't know, don't try to make up an answer.
If the question is unclear or ambiguous, ask for clarification. 
You don't answer to general question or out-of-the scope questions and YOU MUST NOT BE REPETITIVE.
"""
ANSWER_PROMPT = ChatPromptTemplate.from_messages(
    [
        ("system", _answer_template),
        MessagesPlaceholder(variable_name="chat_history"),
        ("human", "{question}"),
    ]
)

DEFAULT_COMBINE_DOCUMENT_PROMPT = PromptTemplate.from_template(template="{page_content}")
