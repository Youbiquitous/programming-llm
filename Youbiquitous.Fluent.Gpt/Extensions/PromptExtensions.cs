///////////////////////////////////////////////////////////////////
//
// PRONTO: GPT-booking proof-of-concept
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//

using Azure.AI.OpenAI;
using Pronto.Gpt.Prompts.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Youbiquitous.Fluent.Gpt.Model;

namespace Youbiquitous.Fluent.Gpt.Extensions
{

    /// <summary>
    /// Provides useful UI helpers on the Prompt class
    /// </summary>
    public static class PromptExtensions
    {
        /// <summary>
        /// Returns a ChatMessage instance from a Prompt object
        /// </summary>
        /// <returns></returns>
        public static IList<ChatRequestMessage> ToChatMessages(this Prompt prompt, params object?[] values)
        {
            var retList = new List<ChatRequestMessage>();
            retList.Add(new ChatRequestSystemMessage(prompt.Build(values)));
            retList.AddRange(prompt.FewShotExamples);

            return retList;
        }

    }
}
