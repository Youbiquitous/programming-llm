from typing import List
from langchain_core.pydantic_v1 import BaseModel, Field

class hypothetical_questions(BaseModel):
    """Generate hypothetical questions"""
    
    questions: List[str] = Field(
        ...,
        description="Hypothetical questions that the below document could be used to answer",
    )