using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SupportPlatform.BLL.Services
{
    public enum BlobContainerType { images, reports, taskcaches };

    public class BlobService : BaseService
    {
        private readonly IConfiguration _configuration;

        public BlobService(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> UploadAsync(IFormFile file, BlobContainerType blobContainer)
        {
            string azureStorageDomain = _configuration["Azure:Storage:Domain"];
            string azureStorageConnStr = _configuration["Azure:Storage:ConnectionString"];
            string blobContainerName = blobContainer.ToString();
            string fileName = Guid.NewGuid().ToString() + "." + Path.GetExtension(file.FileName);

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);

                var containerClient = new BlobContainerClient(azureStorageConnStr, blobContainerName);
                var blobClient = containerClient.GetBlobClient(fileName);
                memoryStream.Position = 0;
                var response = await blobClient.UploadAsync(memoryStream);
            }

            return $"https://{azureStorageDomain}/{blobContainerName}/{fileName}";
        }

        public async Task<string> UploadAsync(string fileName, BlobContainerType blobContainer)
        {
            string azureStorageDomain = _configuration["Azure:Storage:Domain"];
            string azureStorageConnStr = _configuration["Azure:Storage:ConnectionString"];
            string blobContainerName = blobContainer.ToString();

            using (var memoryStream = new MemoryStream())
            {
                File.OpenRead(fileName).CopyTo(memoryStream);

                var containerClient = new BlobContainerClient(azureStorageConnStr, blobContainerName);
                var blobClient = containerClient.GetBlobClient(fileName);
                memoryStream.Position = 0;
                var response = await blobClient.UploadAsync(memoryStream);
            }

            return $"https://{azureStorageDomain}/{blobContainerName}/{fileName}";
        }

        public async Task<string> UploadAsync(string fileName, byte[] data, BlobContainerType blobContainer)
        {
            string azureStorageDomain = _configuration["Azure:Storage:Domain"];
            string azureStorageConnStr = _configuration["Azure:Storage:ConnectionString"];
            string blobContainerName = blobContainer.ToString();

            using (var memoryStream = new MemoryStream(data))
            {
                var containerClient = new BlobContainerClient(azureStorageConnStr, blobContainerName);
                var blobClient = containerClient.GetBlobClient(fileName);
                memoryStream.Position = 0;
                var response = await blobClient.UploadAsync(memoryStream, overwrite: true);
            }

            return $"https://{azureStorageDomain}/{blobContainerName}/{fileName}";
        }

        public string GetSASDownloadUrl(string fileName, BlobContainerType blobContainer)
        {
            string azureStorageConnStr = _configuration["Azure:Storage:ConnectionString"];
            string blobContainerName = blobContainer.ToString();

            var containerClient = new BlobContainerClient(azureStorageConnStr, blobContainerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            var sasUrl = blobClient.GenerateSasUri(Azure.Storage.Sas.BlobSasPermissions.Read, DateTimeOffset.Now.AddHours(1));

            return sasUrl.AbsoluteUri;
        }

        public bool DeleteBlob(string fileName, BlobContainerType blobContainer)
        {
            string azureStorageConnStr = _configuration["Azure:Storage:ConnectionString"];
            string blobContainerName = blobContainer.ToString();

            var containerClient = new BlobContainerClient(azureStorageConnStr, blobContainerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            return blobClient.DeleteIfExists();
        }

        public async Task<string> GetBlobStringData(string fileName, BlobContainerType blobContainer)
        {
            string azureStorageConnStr = _configuration["Azure:Storage:ConnectionString"];
            string blobContainerName = blobContainer.ToString();

            var containerClient = new BlobContainerClient(azureStorageConnStr, blobContainerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            var result = string.Empty;

            if (blobClient.Exists().Value)
            {
                var response = await blobClient.DownloadAsync();
                using (var streamReader = new StreamReader(response.Value.Content))
                {
                    while (!streamReader.EndOfStream)
                    {
                        result += await streamReader.ReadLineAsync();
                    }
                }
            }

            return result;
        }

        public async Task<List<string>> GetBlobList(BlobContainerType blobContainer)
        {
            var result = new List<string>();

            string azureStorageConnStr = _configuration["Azure:Storage:ConnectionString"];
            string blobContainerName = blobContainer.ToString();

            var containerClient = new BlobContainerClient(azureStorageConnStr, blobContainerName);
            var blobs = containerClient.GetBlobsAsync().AsPages(default, 50);

            await foreach (var blobPage in blobs)
            {
                foreach (var blobItem in blobPage.Values)
                {
                    result.Add(blobItem.Name);
                }
            }

            return result;
        }
    }
}
