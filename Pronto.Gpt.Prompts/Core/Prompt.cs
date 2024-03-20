///////////////////////////////////////////////////////////////////
//
// PRONTO: GPT-booking proof-of-concept
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//

using Azure.AI.OpenAI;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pronto.Gpt.Prompts.Core;

public abstract class Prompt
{
    public Prompt(string prompText)
    {
        Text = prompText;
        FewShotExamples = new List<ChatRequestMessage>();
    }
    public Prompt(string prompText, IList<ChatRequestMessage> fewShotExamples, int historyWindow = 0)
    {
        Text = prompText;
        FewShotExamples = fewShotExamples;
        HistoryWindow = historyWindow;
    }

    /// <summary>
    /// Repository for the actual prompt text
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Repository for the few-shot examples
    /// </summary>
    public IList<ChatRequestMessage> FewShotExamples { get; set; }

    /// <summary>
    /// History window (e.g. how many past messages should be brought withing this prompt)
    /// </summary>
    public int HistoryWindow { get; set; }

    /// <summary>
    /// Overridable method to compose the actual prompt
    /// </summary>
    /// <returns></returns>
    public virtual Prompt Format(params object?[] values)
    {
        Text = string.Format(Text, values);
        return this;
    }

    /// <summary>
    /// Overridable method to compose the actual prompt adding input
    /// </summary>
    /// <returns></returns>
    public virtual Prompt AddInput(IEnumerable<string> input)
    {
        Text = Text.Replace("{userInput}", string.Join(" ", input));
        return this;
    }

    /// <summary>
    /// Overridable method to compose the actual prompt
    /// </summary>
    /// <returns></returns>
    public virtual string Build(params object?[] values)
    {
        return string.Format(Text, values);
    }
}