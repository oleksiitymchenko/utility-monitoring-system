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
using Microsoft.EntityFrameworkCore;

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

            var isRegistered = dbContext
               .ControllerRegistry
               .FirstOrDefaultAsync(x => x.Id == Guid.Parse(microcontrollerId))
               != default;
            if (!isRegistered)
            {
                log.LogWarning($"[{microcontrollerId}] Controller not registered");
                return new OkResult();
            }

            var cloudAccount = CloudStorageAccount.Parse(opt.StorageAccountConnectionString);
            var blobClient = cloudAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(opt.ContainerName);
            await container.CreateIfNotExistsAsync();
            var blob = container.GetBlockBlobReference($"{microcontrollerId}-{DateTime.UtcNow}-picture.jpg");

            var rec = new TelemetryRecord
            {
                BlobName = blob.Name,
                ProcessedSuccessful = false,
                ControllerRegistryId = Guid.Parse(microcontrollerId),
                CreatedOn = DateTime.UtcNow,
                ImageUrl = blob.Uri.ToString()
            };
            await dbContext.TelemetryRecord.AddAsync(rec);
            await dbContext.SaveChangesAsync();

            var ms = new MemoryStream();
            await req.Body.CopyToAsync(ms);
            var array = ms.ToArray();
            await blob.UploadFromByteArrayAsync(array, 0, array.Length);
            
            return new OkResult();
        }
    }
}
