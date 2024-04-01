import os
from knowledge_service.storage import StorageService
from chat_service.chat import ChatService
from dotenv import load_dotenv, find_dotenv
import streamlit as st
from htbuilder import a, span
from security.password_check import check_password

def init_env():
    _ = load_dotenv(find_dotenv())

# Initialize the environment
init_env()
deployment_name=os.getenv("AOAI_DEPLOYMENTID", "")
deployment_name_fallback=os.getenv("AOAI_DEPLOYMENTID_FALLBACK", "")
embeddings_deployment_name=os.getenv("AOAI_EMBEDDINGS_DEPLOYMENTID", "")
storage_service = StorageService(deployment=deployment_name, 
                                 embedding_deployment=embeddings_deployment_name, 
                                 fallbacks=[deployment_name_fallback])

st.set_page_config(page_title="Programming LLMs - Chapter 7", page_icon="robot_face")

if not check_password():
    # Do not continue if check_password is not True.
    st.stop()


# Authenticated execution of the app

# Load data into an index 
@st.cache_resource(show_spinner=False)
def load_initial_data():
    with st.spinner(text="Loading and indexing initial documents! This should take a couple of minutes."):
        # remove this if you want to load files anyway
        if not bool(storage_service.get_loaded_files()):
            storage_service.load_pdf_files()
        return storage_service.get_retriever()

retriever = load_initial_data()
chat_engine = ChatService(deployment_name, deployment_name, deployment_name, retriever, fallbacks=[deployment_name_fallback])

# Create a header in the Streamlit app for the AI assistant
st.header("📚Chapter 7 - Chat with Your Data 💬")

# Initialize the chat message history if needed
if "messages" not in st.session_state.keys(): 
    st.session_state.messages = []

# Prompt for user input and save to chat history
if query := st.chat_input("Your question"): 
    st.session_state.messages.append({"role": "user", "content": query})
        
# Display the prior chat history
for message in st.session_state.messages: 
    with st.chat_message(message["role"]):
        st.write(message["content"])
        if "docs" in message and message["docs"]:
            for doc in message["docs"]:
                mention_html = span(
                    span(),
                    "🔗",
                    span(
                        style=(
                            "border-bottom:0.05em solid"
                            " rgba(55,53,47,0.25);font-weight:500;flex-shrink:0"
                        )
                    )(doc.metadata["source"]),
                    span(" page: "),
                    span(
                        style=(
                            "font-weight:bold;"
                        )
                    )(doc.metadata["page"]),
                )
                st.write(str(mention_html), unsafe_allow_html=True)
# If last message is not from assistant, generate a new response
if st.session_state.messages and st.session_state.messages[-1]["role"] != "assistant":
    with st.chat_message("assistant"):
        with st.spinner("Thinking..."):
            response = chat_engine.run_rag(query=query, chat_history=st.session_state.messages)
            # Display the AI-generated answer
            st.write(response["answer"])
            # Display the sources

            # Please note!
            # We are displaying only file name and page of documents retrieved from vector stores
            # It can happen that two different chunks for the same page are taken into consideration
            # For a proper quotation (verbatim, too) feature, see here:
            # https://python.langchain.com/docs/use_cases/question_answering/citations
            
            for doc in response["docs"]:
                mention_html = span(
                    span(),
                    "🔗",
                    span(
                        style=(
                            "border-bottom:0.05em solid"
                            " rgba(55,53,47,0.25);font-weight:500;flex-shrink:0"
                        )
                    )(doc.metadata["source"]),
                    span(" page: "),
                    span(
                        style=(
                            "font-weight:bold;"
                        )
                    )(doc.metadata["page"]),
                )
                st.write(str(mention_html), unsafe_allow_html=True)

            message = {"role": "assistant", "content": response["answer"], "docs": response["docs"]}
            # Add response to message history
            st.session_state.messages.append(message)