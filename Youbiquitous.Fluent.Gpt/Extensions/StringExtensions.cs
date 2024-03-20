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
    /// Provides useful helpers on the String class, with regards to ChatMessages
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Returns a user chat message
        /// </summary>
        /// <returns></returns>
        public static ChatRequestMessage User(this string theMessage)
        {
            return new ChatRequestUserMessage(theMessage);
        }

        /// <summary>
        /// Returns an assistant chat message
        /// </summary>
        /// <returns></returns>
        public static ChatRequestMessage Assistant(this string theMessage)
        {
            return new ChatRequestUserMessage(theMessage);
        }

        /// <summary>
        /// Returns a system chat message
        /// </summary>
        /// <returns></returns>
        public static ChatRequestMessage System(this string theMessage)
        {
            return new ChatRequestUserMessage(theMessage);
        }
    }
}
