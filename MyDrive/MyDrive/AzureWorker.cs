using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace MainService
{
    public class AzureWorker
    {
        private string connStr;
        private string containerName;
        private BlobServiceClient serviceClient;
        private BlobContainerClient containerClient;

        public AzureWorker()
        {
            connStr = Environment.GetEnvironmentVariable("MyDriveBlobConnectionString");
            containerName = Environment.GetEnvironmentVariable("MyDriveContainerName");
            serviceClient = new BlobServiceClient(connStr);
            containerClient = serviceClient.GetBlobContainerClient(containerName);
        }

        public void UploadFile(string filename, Stream content)
        {
            content.Position = 0;
            containerClient.UploadBlob(filename, content);
        }

        public async Task<Byte[]> DownloadFile(string hash)
        {
            var blob = containerClient.GetBlobClient(hash);
            BlobDownloadInfo downloadInfo = await blob.DownloadAsync();
            using (var memoryStream = new MemoryStream())
            {
                downloadInfo.Content.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public void DeleteFile(string url)
        {
        }
    }
}
