///////////////////////////////////////////////////////////////////
//
// PRONTO: GPT-booking proof-of-concept
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//


using Microsoft.AspNetCore.Mvc;
using Pronto.Shared.Settings;

namespace Pronto.App.Controllers;

public class ProntoController : Controller
{
    public ProntoController(AppSettings settings, IHttpContextAccessor accessor)
    {
        Settings = settings;
        HttpConnection = accessor;
    }

    /// <summary>
    /// Current server 
    /// </summary>
    protected string ServerUrl => $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

    /// <summary>
    /// Gain access to application settings
    /// </summary>
    protected AppSettings Settings { get; }

    /// <summary>
    /// Gain access to HTTP connection info
    /// </summary>
    protected IHttpContextAccessor HttpConnection { get; }

}