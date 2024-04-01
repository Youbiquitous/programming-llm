///////////////////////////////////////////////////////////////////
//
// Internal GPT wrapper API
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//


using Azure.AI.OpenAI;
using System.Collections.Generic;

namespace Youbiquitous.Fluent.Gpt.Providers;

/// <summary>
/// Stores past chat messages in memory
/// </summary>
public class InMemoryHistoryProvider : IHistoryProvider<(string, string)>
{
    private Dictionary<(string, string),IList<ChatRequestMessage>> _list;

    public InMemoryHistoryProvider()
    {
        Name = "In-Memory";
        _list = new Dictionary<(string, string), IList<ChatRequestMessage>>();
    }

    /// <summary>
    /// Name of the provider
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Retrieve the stored list of chat messages
    /// </summary>
    /// <returns></returns>
    public IList<ChatRequestMessage> GetMessages(string userId, string queue)
    {
        return GetMessages((userId, queue));
    }

    /// <summary>
    /// Retrieve the stored list of chat messages
    /// </summary>
    /// <returns></returns>
    public IList<ChatRequestMessage> GetMessages((string, string) userId)
    {
        return _list.TryGetValue(userId, out var messages)
            ? messages
            : new List<ChatRequestMessage>();
    }


    /// <summary>
    /// Save a new list of chat messages
    /// </summary>
    /// <returns></returns>
    public bool SaveMessages(IList<ChatRequestMessage> messages, string userId, string queue)
    {
        return SaveMessages(messages, (userId, queue));
    }

    /// <summary>
    /// Save a new list of chat messages
    /// </summary>
    /// <returns></returns>
    public bool SaveMessages(IList<ChatRequestMessage> messages, (string, string) userInfo)
    {
        if(_list.ContainsKey(userInfo))
            _list[userInfo] = messages;
        else
            _list.Add(userInfo, messages);
        return true;
    }

    /// <summary>
    /// Clear list of chat messages
    /// </summary>
    /// <returns></returns>
    public bool ClearMessages(string userId, string queue)
    {
        return ClearMessages((userId, queue));
    }

    /// <summary>
    /// Clear list of chat messages
    /// </summary>
    /// <returns></returns>
    public bool ClearMessages((string userId, string queue) userInfo)
    {
        if (_list.ContainsKey(userInfo))
            _list.Remove(userInfo);
        return true;
    }
}