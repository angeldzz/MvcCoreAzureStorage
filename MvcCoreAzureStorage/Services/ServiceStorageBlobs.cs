using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using MvcCoreAzureStorage.Models;

namespace MvcCoreAzureStorage.Services
{
    public class ServiceStorageBlobs
    {
        private BlobServiceClient client;

        public ServiceStorageBlobs(BlobServiceClient client)
        {
            this.client = client;
        }

        public async Task<List<string>> GetBlobContainersAsync()
        {
            List<string> containers = new List<string>();
            await foreach (var container in this.client.GetBlobContainersAsync())
            {
                containers.Add(container.Name);
            }

            return containers;
        }

        //METODO PARA CREAR UN CONTAINER
        public async Task CreateContainerAsync(string containerName)
        {
            await this.client.CreateBlobContainerAsync(containerName.ToLower(), PublicAccessType.Blob);
        }

        public async Task DeleteContainerAsync(string containerName)
        {
            await this.client.DeleteBlobContainerAsync(containerName);
        }

        //UN LISTADO DE FICHERO DENTRO DE UN CONTAINER
        public async Task<List<BlobModel>> GetBlobsAsync(string containerName)
        {
            BlobContainerClient containerClient = this.client.GetBlobContainerClient(containerName);
            BlobContainerProperties properties = await containerClient.GetPropertiesAsync();
            bool isPrivateContainer = properties.PublicAccess == PublicAccessType.None;

            List<BlobModel> models = new List<BlobModel>();
            await foreach (BlobItem item in containerClient.GetBlobsAsync())
            {
                BlobClient blobClient = containerClient.GetBlobClient(item.Name);
                BlobModel model = new BlobModel();
                model.Nombre = item.Name;
                model.Url = this.GetBlobUrl(blobClient, isPrivateContainer);
                model.Container = containerName;
                models.Add(model);
            }

            return models;
        }

        public async Task DeleteBlobAsync(string containerName, string blobName)
        {
            BlobContainerClient containerClient = this.client.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.DeleteAsync();
        }

        public async Task UploadBlobAsync(string containerName, string blobName, Stream stream)
        {
            BlobContainerClient containerClient = this.client.GetBlobContainerClient(containerName);
            await containerClient.UploadBlobAsync(blobName, stream);
        }

        private string GetBlobUrl(BlobClient blobClient, bool isPrivateContainer)
        {
            if (!isPrivateContainer)
            {
                return blobClient.Uri.ToString();
            }

            if (blobClient.CanGenerateSasUri)
            {
                BlobSasBuilder sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = blobClient.BlobContainerName,
                    BlobName = blobClient.Name,
                    Resource = "b",
                    ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
                };
                sasBuilder.SetPermissions(BlobSasPermissions.Read);

                return blobClient.GenerateSasUri(sasBuilder).ToString();
            }

            return blobClient.Uri.ToString();
        }
    }
}