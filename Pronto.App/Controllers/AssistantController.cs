///////////////////////////////////////////////////////////////////
//
// PRONTO: GPT-booking proof-of-concept
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//


using Microsoft.AspNetCore.Mvc;
using Pronto.App.Application;
using Pronto.App.Models;
using Pronto.Shared.Settings;
using Youbiquitous.Fluent.Gpt.Providers;

namespace Pronto.App.Controllers;

public partial class AssistantController : ProntoController
{
    private readonly AssistantGptService _apiGpt;
    private readonly InMemoryHistoryProvider _historyProvider;
    public AssistantController(AppSettings settings, IHttpContextAccessor accessor, InMemoryHistoryProvider historyProvider)
        : base(settings, accessor)
    {
        _apiGpt = new AssistantGptService(Settings);
        _historyProvider = historyProvider;
    }

    /// <summary>
    /// Home page  
    /// </summary>
    /// <returns></returns>
    public IActionResult Index()
    {
        //Dirty patch to prevent HttpContext.Session.Id from changing
        HttpContext.Session.SetString("Something", "Something");

        var vm = _apiGpt.GetChatViewModel(_historyProvider.GetMessages(HttpContext.Session.Id, "ApiDoc"), "ApiDoc");
        vm.Header = "Project Manager Assistant";
        return View(vm);
    }
}