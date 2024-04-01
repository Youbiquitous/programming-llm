"""
Contains a class to handle the authentication
"""

import hmac
import streamlit as st

def check_password():
    def password_entered():
        if hmac.compare_digest(st.session_state["password"], st.secrets["password"]):
            st.session_state["password_correct"] = True
            # No need to store the password
            del st.session_state["password"]
        else:
            st.session_state["password_correct"] = False

    # Return True if the password has been validated
    if st.session_state.get("password_correct", False):
        return True

    # Show input for password
    st.text_input(
        "Password", type="password", on_change=password_entered, key="password"
    )
    if "password_correct" in st.session_state:
        st.error("😕 Password incorrect")
    return False