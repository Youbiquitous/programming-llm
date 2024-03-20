///////////////////////////////////////////////////////////////////
//
// PRONTO: GPT-booking proof-of-concept
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//


namespace Pronto.Shared.Settings;

public class GeneralSettings
{
    /// <summary>
    /// Project nme
    /// </summary>
    public string ProjectName { get; set; }

    /// <summary>
    /// Application primary name
    /// </summary>
    public string ApplicationName { get; set; }

    /// <summary>
    /// Application description
    /// </summary>
    public string ApplicationDescription { get; set; }

    /// <summary>
    /// Indicates the version of the application 
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    /// Indicates the build display information 
    /// </summary>
    public string BuildInfo { get; set; }

    /// <summary>
    /// Database secrets
    /// </summary>
    public SecretsSettings Secrets { get; set; }

    /// <summary>
    /// OpenAI subscription secrets
    /// </summary>
    public OpenAISettings OpenAI { get; set; }
}