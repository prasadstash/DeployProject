using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DeployProject.Interfaces;


namespace DeployProject.Services
{
    public class BlobService : IBlobService
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        public BlobService(IConfiguration configuration)
        {
            _connectionString = configuration["AzureBlobStorage:ConnectionString"];
            _containerName = configuration["AzureBlobStorage:ContainerName"];
        }

        public async Task<string> UploadFileAsync(IFormFile file, string fileName)
        {
            var blobClient = new BlobContainerClient(_connectionString, _containerName);
            await blobClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blob = blobClient.GetBlobClient(fileName);

            using (var stream = file.OpenReadStream())
            {
                await blob.UploadAsync(stream, overwrite: true);
            }

            return blob.Uri.ToString();
        }
    }
}
