///////////////////////////////////////////////////////////////////
//
// PRONTO: GPT-booking proof-of-concept
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//


using Pronto.Shared.Settings;

namespace Pronto.App.Models;

public class MainViewModelBase
{
    public static MainViewModelBase Default(AppSettings settings, string title = "PRONTO-GPT")
    {
        return new MainViewModelBase(title, settings);
    }

    public MainViewModelBase(string title, AppSettings settings)
    {
        Title = title;
        Settings = settings;
    }

    public MainViewModelBase(AppSettings settings, string title = "PRONTO-GPT")
    {
        Title = title;
        Settings = settings;
    }

    public string Title { get; set; }
    public AppSettings Settings { get; set; }
    public string Header { get; set; }
}