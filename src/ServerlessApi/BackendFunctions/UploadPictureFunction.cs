using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServerlessApi;
using Microsoft.WindowsAzure.Storage;
using ServerlessApi.Context;
using System.Text.Json;
using System.Linq;

namespace BackendFunctions
{
    public class UploadPictureFunction
    {
        private readonly FunctionOptions opt;
        private readonly MonitoringDbContext dbContext;

        public UploadPictureFunction(
            IOptions<FunctionOptions> options,
             MonitoringDbContext dbContext)
        {
            this.opt = options.Value;
            this.dbContext = dbContext;
        }

        [FunctionName("upload-blob")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "upload-blob/{microcontrollerId}")] HttpRequest req,
            string microcontrollerId,
            ILogger log)
        {
            if (string.IsNullOrEmpty(microcontrollerId))
            {
                var message = "Request does not contain controller id";
                log.LogWarning(message);
                return new ObjectResult(new ErrorModel { StatusCode = 400, Message = message }) { StatusCode = 400 };
            }

            if (req.ContentLength <= 1)
            {
                var message = $"[{microcontrollerId}] Request does not payload";
                log.LogWarning(message);
                return new ObjectResult(new ErrorModel { StatusCode = 400, Message = message }) { StatusCode = 400 };
            }

            log.LogInformation($"[{microcontrollerId}]");

            var isRegistered = dbContext
               .ControllerRegistry
               .FirstOrDefault(x => x.Id == Guid.Parse(microcontrollerId))
               != default;
            if (!isRegistered)
            {
                log.LogWarning($"[{microcontrollerId}] Controller not registered");
                return new OkResult();
            }
            log.LogInformation($"Registered");


            log.LogInformation($"[{opt.StorageAccountConnectionString}]");

            var cloudAccount = CloudStorageAccount.Parse(opt.StorageAccountConnectionString);
            log.LogInformation($"Created acccount");

            var blobClient = cloudAccount.CreateCloudBlobClient();
            log.LogInformation($"blobClient");

            var container = blobClient.GetContainerReference(opt.ContainerName);
            await container.CreateIfNotExistsAsync();
            var blob = container.GetBlockBlobReference($"{microcontrollerId}-{DateTime.UtcNow}-picture.jpg");
            log.LogInformation($"blob");

            var rec = new TelemetryRecord
            {
                BlobName = blob.Name,
                ProcessedSuccessful = false,
                ControllerRegistryId = Guid.Parse(microcontrollerId),
                CreatedDate = DateTime.UtcNow,
                ImageUrl = blob.Uri.ToString()
            };
            dbContext.TelemetryRecord.Add(rec);
            dbContext.SaveChanges();
            log.LogInformation("Added record to context {rec}", JsonSerializer.Serialize(rec, new JsonSerializerOptions { MaxDepth = 3 }));
            var ms = new MemoryStream();
            req.Body.CopyTo(ms);
            var array = ms.ToArray();
            await blob.UploadFromByteArrayAsync(array, 0, array.Length);
            log.LogInformation("Uploaded blob");

            return new OkResult();
        }
    }
}
