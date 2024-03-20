///////////////////////////////////////////////////////////////////
//
// PRONTO: GPT-booking proof-of-concept
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//


using Azure.AI.OpenAI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Azure;
using System.Text;
using Youbiquitous.Fluent.Gpt.Extensions;
using Youbiquitous.Fluent.Gpt.Providers;

namespace Pronto.App.Controllers;

public partial class AssistantController 
{
    /// <summary>
    /// Clear previous history
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Route("/chat/clear/")]
    public IActionResult Clear()
    {
        //Clear history
        var history = _historyProvider.ClearMessages(HttpContext.Session.Id, "Assistant");

        return Json(history);
    }

    /// <summary>
    /// Receives and handles a message from the user
    /// </summary>
    /// <param name="email"></param>
    /// <param name="destinationLanguage"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("/translate")]
    public IActionResult Translate(
        [Bind(Prefix = "email")] string email,
        [Bind(Prefix = "destination")] string destinationLanguage)
    {
        // Place a call to GPT
        var text = _apiGpt.Translate(email, destinationLanguage);
        return Json(text);
    }

    /// <summary>
    /// Receives and handles a message from the user
    /// </summary>
    /// <param name="message"></param>
    /// <param name="origEmail"></param>
    /// <returns></returns>
    [Route("/chat/message/")]
    public async Task<IActionResult> Message(
        [Bind(Prefix="msg")] string message,
        [Bind(Prefix = "orig")] string origEmail = "")
    {
        try
        {
            // Set response headers for SSE
            HttpContext.Response.Headers.Add("Content-Type", "text/event-stream");
            HttpContext.Response.Headers.Add("Cache-Control", "no-cache");
            HttpContext.Response.Headers.Add("Connection", "keep-alive");

            var writer = new StreamWriter(HttpContext.Response.Body);

            // Retrieve history
            var history = _historyProvider.GetMessages(HttpContext.Session.Id, "Assistant");
            // Place a call to GPT
            var streaming = await _apiGpt.HandleStreamingMessage(message, history, origEmail);
            // Start streaming to the client
            var chatResponseBuilder = new StringBuilder();
            await foreach (var chatMessage in streaming)
            {
                chatResponseBuilder.AppendLine(chatMessage.ContentUpdate);
                await writer.WriteLineAsync($"data: {chatMessage.ContentUpdate}\n\n");
                await writer.FlushAsync();
            }
            // Close the SSE connection after sending all messages
            await Response.CompleteAsync();

            //Update history if streaming completed
            history.Add(chatResponseBuilder.ToString().User());
            _historyProvider.SaveMessages(history, HttpContext.Session.Id, "Assistant");

            return new EmptyResult();
        }
        catch (Exception ex)
        {
            // Handle exceptions and return an error response
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
        
    }
}