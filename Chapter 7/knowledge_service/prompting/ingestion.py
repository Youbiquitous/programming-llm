from langchain_core.prompts.chat import ChatPromptTemplate

FAQ_PROMPT = ChatPromptTemplate.from_template("Generate a list of 4 hypothetical questions that \
                                              the below document could be used to answer \
                                              in the same language as the document:\n\n{doc}")


SUMMARY_PROMPT = ChatPromptTemplate.from_template("You are an assistant tasked with summarizing tables and text. \
                                                  Give a concise summary of the table or text. \
                                                  Table or text chunk: \n\n{doc}")
