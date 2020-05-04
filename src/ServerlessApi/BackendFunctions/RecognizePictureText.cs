using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BackendFunctions
{
    public static class RecognizePictureText
    {
        [FunctionName("RecognizePictureText")]
        public static void Run(
            [BlobTrigger("functions-pictures/{name}.jpg", Connection = "StorageAccountConnectionString")]Stream myBlob, 
            string name, 
            ILogger log)
        {

            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        }
    }
}
