
using OfficeOpenXml;
using System.Text;
using Microsoft.Extensions.Configuration;
using Pronto.Shared.Settings;
using Youbiquitous.Fluent.Gpt;
using Pronto.Infrastructure.Repository;
using Pronto.Infrastructure.Models;
using Pronto.Gpt.Prompts;

// Prepare settings
IConfigurationRoot _configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

var _settings = new AppSettings();
_configuration.Bind(_settings);
ProntoContext.ConnectionString = _settings.General.Secrets.SqlConnectionString;

//Start app
Console.WriteLine("Insert the path");
var filePath = Console.ReadLine().Replace("\"", "");
var _repository = new CassandraRepository();

using (FileStream fs = new(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
{
    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

    using var package = new ExcelPackage(fs);
    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

    int rowCount = worksheet.Dimension.Rows;
    int colCount = worksheet.Dimension.Columns;

    var embeddings = new List<(CassandraChunk, float[])>();
    for (int row = 2; row <= 15; row++)
    {
        Console.WriteLine($"Preparing row #{row - 1}");
        var rowContent = "";
        for (int col = 1; col <= colCount; col++)
        {
            rowContent += $"{worksheet.Cells[1, col].Value?.ToString()}: {worksheet.Cells[row, col].Value?.ToString() ?? "-"} \n";
        }
        var embeddingResult = GptEmbeddingEngine
                        .Using(_settings.General.OpenAI.ApiKey, _settings.General.OpenAI.BaseUrl)
                        .ConversationalModel(_settings.General.OpenAI.DeploymentId)
                        .EmbeddingModel(_settings.General.OpenAI.EmbeddingDeploymentId)
                        .Manipulate("Sei un esperto di cybersecurity in settore sanitario. " +
                        "Scrivi un resoconto della seguente riga in linguaggio fluente." +
                        "Inserisci sempre tutti i possibili riferimenti normativi citati, per facilitarne la ricerca." +
                        "Inserisci tutte le informazioni importanti, sia per la profilazione legale, sia per l'aspetto implementativo." +
                        "Inserisci sempre il riferimento all'ID della misura.")
                        .Input(rowContent)
                        //TODO add reference to complex object + properties to embedd
                        .Embed()
                        .GetResult();
        embeddings.Add((new CassandraChunk(embeddingResult.Text), embeddingResult.Embedding));
    }
    Console.WriteLine($"Starting chunks + embeddings import");
    var output = _repository.AddInitialEmbeddings(embeddings);
    Console.WriteLine($"Imported {output} chunks");
}