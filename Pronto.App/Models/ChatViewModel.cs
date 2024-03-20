///////////////////////////////////////////////////////////////////
//
// PRONTO: GPT-booking proof-of-concept
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//


using Azure.AI.OpenAI;
using Pronto.Shared.Settings;
using Youbiquitous.Fluent.Gpt.Providers;

namespace Pronto.App.Models;

public class ChatViewModel:MainViewModelBase
{
	public ChatViewModel(string title, AppSettings settings, IList<ChatRequestMessage> history) :base(title, settings)
	{
		History = history;
	}
    public ChatViewModel(AppSettings settings, IList<ChatRequestMessage> history, string queue) : base(settings)
    {
        History = history;
        Queue = queue;
    }
    public IList<ChatRequestMessage> History { get; set; }
    public string Queue { get; set; }
}