# Chapter 7 - Chat With Your Data (RAG)

The example application, built in Chapter 7, aims to explore the RAG (Retrieval Augmented Generation) pattern. To do this, a Streamlit app is built.

To use it, follow these 4 steps:

1. **Set Environment Variables**:
   - Configure environment variables with necessary secrets obtained from Azure and potentially from Langsmith for tracing purposes. Replace placeholder values in a `.env.sample` file with actual secrets and rename the file to `.env`.

2. **Set Up a Virtual Environment** (Optional):
   - This step is optional but recommended for managing dependencies cleanly. First, create a virtual environment using the `venv` module in Python. Then, activate the virtual environment. Finally, install the required dependencies listed in the `requirements.txt` file using pip. Example commands:
     ```
     python -m venv /path/to/new/virtual/environment
     source /path/to/new/virtual/environment/bin/activate  # For Unix/Linux
     /path/to/new/virtual/environment/Scripts/Activate.ps1  # For Windows PowerShell
     pip install -r requirements.txt
     ```

3. **Prepare Data Files**:
   - Place the files you want to load and base the responses of the chatbot on in the `/data` folder. The application natively supports PDF files, but you can extend support to XML and CSV files by adding a simple method.

4. **Run the Application**:
   - After activating the virtual environment (if used), run the Streamlit application by executing `streamlit run chat.py` in the terminal. This command launches the Streamlit application and starts serving it locally.


