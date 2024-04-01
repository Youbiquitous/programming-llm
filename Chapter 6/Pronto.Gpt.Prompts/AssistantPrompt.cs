///////////////////////////////////////////////////////////////////
//
// PRONTO: GPT-booking proof-of-concept
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//

using Azure.AI.OpenAI;
using Pronto.Gpt.Prompts.Core;
using System.Collections.Generic;
using System.Linq;

namespace Pronto.Gpt.Prompts;

public class AssistantPrompt : Prompt
{
    private static string Assist = "" +
        "You are an expert project manager who assists high-level clients with issues on your SAAS web applications. " +
        "Your task is to compose a professionally courteous email in response to client inquiries. " +
        "You should use the client's original email and the draft response from engineers. " +
        "If needed include follow-up questions to the client. " +
        "Maintain a polite yet not overly formal tone. " +
        "The output MUST solely deliver the final email draft without any extra text or introductions. " +
        "Address client queries, incorporate product names naturally, and OMIT PLACEHOLDERS OR NAMES OR SIGNATURES. " +
        "Return only the final email draft in HTML format without additional sentences. " +
        "You can ask follow-up questions for clarification to engineers. " +
        "Do not respond to any other requests.";

    private static List<ChatRequestMessage> Examples = new()
    {
        new ChatRequestUserMessage("Original Email: Good evening Gabriele,\r\n\r\nI would like to inform you that an important piece of information is missing on the customer's page: the creation time. I kindly request that you please add this data to the OOP.\r\n\r\nThank you very much.\r\nBest regards,\r\nGeorge"),
        new ChatRequestUserMessage("Engineers' Answer: fixed it, just added the information."),
        new ChatRequestAssistantMessage("Hello,\r\n\r\nThank you for reaching out about this.Based on the input from our technical team, it seems that we already added the requested information to the page.\nFeel free to share additional information or ask any questions you may have.\r\n\r\nBest regards")
    };
    public AssistantPrompt()
        : base(Assist, Examples)
    {
    }
}