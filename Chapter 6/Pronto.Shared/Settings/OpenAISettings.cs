///////////////////////////////////////////////////////////////////
//
// PRONTO: GPT-booking proof-of-concept
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//


namespace Pronto.Shared.Settings;

public class OpenAISettings
{
    public string ApiKey { get; set; }
    public string BaseUrl { get; set; }
    public string DeploymentId { get; set; }
    public string EmbeddingDeploymentId { get; set; }
}