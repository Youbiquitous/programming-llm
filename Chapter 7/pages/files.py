import os
from knowledge_service.storage import StorageService
from security.password_check import check_password
from dotenv import load_dotenv, find_dotenv
import streamlit as st

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

st.title("Loaded files")
for file in storage_service.get_loaded_files():
    col1, col2 = st.columns(2)
    file_name = col1.empty()
    file_name.write(f'{file}')
    delete_button = col2.empty()
    do_action = delete_button.button("Remove", key=file)
    if do_action:
        storage_service.remove_loaded_file(file)
        #  remove row
        file_name.empty()
        delete_button.empty()