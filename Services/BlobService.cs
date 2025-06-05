using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using DeployProject.Interfaces;



namespace DeployProject.Services
{
    public class BlobService : IBlobService
    {
        private readonly string _connectionString;
        private readonly string _containerName = "deployproject";
        private readonly string _accountName;
        private readonly string _accountKey;

        public BlobService(IConfiguration configuration)
        {
            _connectionString = configuration["AzureStorage:ConnectionString"];
            _containerName = configuration["AzureStorage:ContainerName"];
            _accountName = configuration["AzureStorage:AccountName"];
            _accountKey = configuration["AzureStorage:AccountKey"];

        }

        public async Task<string> UploadFileAsync(IFormFile file, string fileName)
        {
            var blobClient = new BlobContainerClient(_connectionString, _containerName);
            //await blobClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
            await blobClient.CreateIfNotExistsAsync();

            var blob = blobClient.GetBlobClient(fileName);

            using (var stream = file.OpenReadStream())
            {
                var x = await blob.UploadAsync(stream, overwrite: true);
               
            }

            return blob.Uri.ToString();
        }

        public string GetTemporaryAccessUrl(string fileName)
        {
           
            // Create the credential
            var credential = new StorageSharedKeyCredential(_accountName, _accountKey);

            // Create the SAS builder
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _containerName,
                BlobName = fileName,
                Resource = "b", // b = blob
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(10)
            };

            // Set read permission
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            // Generate the URI
            var blobUri = new Uri($"https://{_accountName}.blob.core.windows.net/{_containerName}/{fileName}");
            var sasToken = sasBuilder.ToSasQueryParameters(credential).ToString();

            var fullUri = $"{blobUri}?{sasToken}";
            return fullUri;
        }

    }
}
