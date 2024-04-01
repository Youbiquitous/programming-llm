///////////////////////////////////////////////////////////////////
//
// PRONTO: GPT-booking proof-of-concept
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//

namespace Pronto.Shared.Settings;

public class AppSettings
{
    public const string AuthCookieName = "PRONTO.Auth";
    public const string CultureCookieName = "PRONTO.Culture";
    public static readonly string AppName = "PRONTO";

    public AppSettings()
    {
        General = new GeneralSettings();
    }

    /// <summary>
    /// All general settings go here
    /// </summary>
    public GeneralSettings General { get; set; }

    /// <summary>
    /// All secrets settings go here
    /// </summary>
    public SecretsSettings Secrets { get; set; }
}