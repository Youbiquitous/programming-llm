///////////////////////////////////////////////////////////////////
//
// PRONTO: GPT-booking proof-of-concept
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//


namespace Pronto.Shared.Settings;

public class SecretsSettings
{
    /// <summary>
    /// SQL connection string
    /// </summary>
    public string SqlConnectionString { get; set; }

    /// <summary>
    /// OpenAI subscription secrets
    /// </summary>
    public OpenAISettings OpenAI { get; set; }
}