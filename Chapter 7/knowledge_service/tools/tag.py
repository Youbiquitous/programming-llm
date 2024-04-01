from typing import Literal
from langchain_core.pydantic_v1 import BaseModel, Field

class Properties(BaseModel):
    author: str = Field(description="Author of the document")
    confidentiality: Literal["confidential", "public"]