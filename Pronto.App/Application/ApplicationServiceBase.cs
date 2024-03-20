///////////////////////////////////////////////////////////////////
//
// PRONTO: GPT-booking proof-of-concept
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//


using Pronto.Shared.Settings;

namespace Pronto.App.Application;

public class ApplicationServiceBase
{
    public ApplicationServiceBase()
    {
        
    }
    public ApplicationServiceBase(AppSettings settings)
    {
        Settings = settings;
    }

    public AppSettings Settings { get; }
}