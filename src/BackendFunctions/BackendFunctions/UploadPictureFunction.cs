using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Options;

namespace BackendFunctions
{
    public class UploadPictureFunction
    {
        private readonly FunctionOptions _opt;

        public UploadPictureFunction(IOptions<FunctionOptions> options)
        {
            _opt = options.Value;
        }

        [FunctionName("upload-blob")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "upload-blob/{microcontrollerId}")] HttpRequest req,
            string microcontrollerId,
            ILogger log)
        {
            log.LogInformation($"ContentLength: {req.ContentLength}");
            if (req.ContentLength <= 1)
            {
                return new OkResult();
            }

            var cloudAccount = CloudStorageAccount.Parse(_opt.StorageAccountConnectionString);
            var blobClient = cloudAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(_opt.ContainerName);
            await container.CreateIfNotExistsAsync();
            var blob = container.GetBlockBlobReference($"{microcontrollerId}-{DateTime.UtcNow}-picture.jpg");
            var ms = new MemoryStream();
            await req.Body.CopyToAsync(ms);
            var array = ms.ToArray();
            log.LogInformation("Byte array length" + array.Length.ToString());
            await blob.UploadFromByteArrayAsync(array, 0, array.Length);
            
            return new OkResult();
        }
    }
}
