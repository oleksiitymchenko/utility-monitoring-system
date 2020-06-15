using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage.Blob;
using ServerlessApi.Context;
using ServerlessApi.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BackendFunctions
{
    public class RecognizePictureText
    {
        private readonly MonitoringDbContext dbContext;
        private readonly ILogger<RecognizePictureText> log;
        private readonly FunctionOptions opt;

        public RecognizePictureText(
            MonitoringDbContext dbContext,
            IOptions<FunctionOptions> options,
            ILogger<RecognizePictureText> log)
        {

            this.dbContext = dbContext;
            this.log = log;
            this.opt = options.Value;
        }

        [FunctionName("RecognizePictureText")]
        public async Task Run(
            [BlobTrigger("functions-pictures/{pictureName}.jpg", Connection = "StorageAccountConnectionString")]CloudBlockBlob blob,
            string pictureName)
        {
            try
            {
                var client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(opt.CvKey))
                { Endpoint = opt.CvEndpoint };
                var results = await GetRecognitionResult(client, blob.Uri.ToString());
                var targetCounterValue = SelectValueAccordingToCounterType(results);

                log.LogDebug(targetCounterValue);

                if (!string.IsNullOrEmpty(targetCounterValue))
                {
                    var entry = await dbContext
                        .TelemetryRecord
                        .FirstOrDefaultAsync(x =>
                            x.BlobName == pictureName
                            && x.ProcessedSuccessful == false);
                    if (entry == default) 
                    {
                        await dbContext.TelemetryRecord.AddAsync(new TelemetryRecord
                        {
                            ProcessedSuccessful = true,
                            CounterValue = int.Parse(targetCounterValue),
                            CreatedDate = DateTime.UtcNow,
                            ImageUrl = blob.Uri.ToString(),
                            BlobName = pictureName
                        });
                    }
                    else
                    {
                        entry.CounterValue = int.Parse(targetCounterValue);
                        entry.ProcessedSuccessful = true;
                        dbContext.Update(entry);
                    }
                    
                    await dbContext.SaveChangesAsync();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private string SelectValueAccordingToCounterType(List<TextRecognitionResult> results)
        {
            var flatList = results
                    .SelectMany(x => x.Lines)
                    .Select(line => Regex.Replace(line.Text, @"[^0-9a-zA-Z]+", ""));

            var targetCounterValue = flatList
                .FirstOrDefault(line => line.Length >= 5 && line.IsDigitsOnly());
            targetCounterValue = targetCounterValue.Substring(0, 5);
            return targetCounterValue;
        }

        private async Task<List<TextRecognitionResult>> GetRecognitionResult(ComputerVisionClient client, string urlImage)
        {
            log.LogDebug("Batch read file by url - {url}", urlImage);

            BatchReadFileHeaders textHeaders = await client.BatchReadFileAsync(urlImage);
            string operationLocation = textHeaders.OperationLocation;
            const int numberOfCharsInOperationId = 36;
            string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

            // Extract the text
            // Delay is between iterations and tries a maximum of 10 times.
            int i = 0;
            int maxRetries = 10;
            ReadOperationResult results;
            log.LogDebug($"Extracting text from URL image {Path.GetFileName(urlImage)}...");
            do
            {
                results = await client.GetReadOperationResultAsync(operationId);
                Console.WriteLine("Server status: {0}, waiting {1} seconds...", results.Status, i);
                await Task.Delay(1000);
                if (i == 9)
                {
                    Console.WriteLine("Server timed out.");
                }
            }
            while ((results.Status == TextOperationStatusCodes.Running ||
                results.Status == TextOperationStatusCodes.NotStarted) && i++ < maxRetries);
            // Display the found text.
            Console.WriteLine();
            var textRecognitionLocalFileResults = results.RecognitionResults;
            foreach (TextRecognitionResult recResult in textRecognitionLocalFileResults)
            {
                foreach (Line line in recResult.Lines)
                {
                    Console.WriteLine(line.Text);
                }
            }
            Console.WriteLine();
            return results.RecognitionResults.ToList();
        }
    }
}
