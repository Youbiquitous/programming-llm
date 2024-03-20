///////////////////////////////////////////////////////////////////
//
// PRONTO: GPT-booking proof-of-concept
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//

using Azure.AI.OpenAI;

namespace Youbiquitous.Fluent.Gpt.Extensions
{

    /// <summary>
    /// Provides useful UI helpers on the ChatMessageClass
    /// </summary>
    public static class ChatMessageExtensions
    {
        /// <summary>
        /// Returns colors for the chat widget based on the role of the message
        /// </summary>
        /// <returns></returns>
        public static string ChatColors(this ChatRequestMessage message)
        {
            return message.Role == ChatRole.User
                ? "col-md-10 col-lg-4"
                : "col-md-10 col-lg-7 bg-mediumgray";
        }

        /// <summary>
        /// Returns colors for the chat widget based on the role of the message
        /// </summary>
        /// <returns></returns>
        public static string Content(this ChatRequestMessage message)
        {
            if (message.Role == ChatRole.User)
                return ((ChatRequestUserMessage)message).Content;
            else if (message.Role == ChatRole.Assistant)
                return ((ChatRequestAssistantMessage)message).Content;
            else if (message.Role == ChatRole.System)
                return ((ChatRequestSystemMessage)message).Content;
            else if (message.Role == ChatRole.Tool)
                return ((ChatRequestToolMessage)message).Content;
            else if (message.Role == ChatRole.Function)
                return ((ChatRequestFunctionMessage)message).Content;
            else
                return string.Empty;
        }

        /// <summary>
        /// Returns the alignment for the chat widget based on the role of the message
        /// </summary>
        /// <returns></returns>
        public static string ChatAlignment(this ChatRequestMessage message)
        {
            return message.Role == ChatRole.User 
                ? "justify-content-end" 
                : "justify-content-start";
        }

    }
}
