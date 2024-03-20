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

public partial class AccountController : ProntoController
{
    public AccountController(AppSettings settings, IHttpContextAccessor accessor)
        : base(settings, accessor)
    {

    }
}