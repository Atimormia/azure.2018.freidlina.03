using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureWorks.Services.Common
{
    public class AzureFileLoader : IFileLoader
    {
        private readonly CloudBlobClient _blobClient;
        private readonly CloudQueueClient _queueClient;

        public AzureFileLoader()
        {
            _blobClient = ServiceConfigurator.GetStorage().CreateCloudBlobClient();
            _queueClient = ServiceConfigurator.GetStorage().CreateCloudQueueClient();
        }

        public async Task UploadFile(byte[] bytes)
        {
            var container = _blobClient.GetContainerReference("adventure-works-files");
            var filename = $"{DateTime.Now:s}_{Guid.NewGuid()}.docx";
            var blob = container.GetBlockBlobReference(filename);
            blob.UploadFromByteArray(bytes, 0, bytes.Length);

            await _queueClient.GetQueueReference("adventure-works-queue")
                .AddMessageAsync(new CloudQueueMessage($"File {filename} was uploaded"));
        }
    }
}
