using Azure.Storage.Blobs;
using FujiyBlog.Core.EntityFramework;
using System.IO;
using System.Threading.Tasks;

namespace FujiyBlog.Core.Services
{
    public class AzureStorageFileUploadService : IFileUploadService
    {
        private readonly string connectionString;
        private readonly string uploadContainerName;

        public AzureStorageFileUploadService(SettingRepository settings)
        {
            connectionString = $"DefaultEndpointsProtocol=https;AccountName={settings.AzureStorageAccountName};AccountKey={settings.AzureStorageAccountKey}";
            uploadContainerName = settings.AzureStorageUploadContainerName;
        }

        public async Task<string> UploadFile(Stream fileStream, string filePath)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(uploadContainerName);

            await containerClient.CreateIfNotExistsAsync();

            BlobClient blobClient = containerClient.GetBlobClient(filePath);

            if (await blobClient.ExistsAsync())
            {
                string folder = Path.GetDirectoryName(filePath);
                string originalFileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                string extension = Path.GetExtension(filePath);

                int fileCount = 2;
                while (await blobClient.ExistsAsync())
                {
                    string fileName = originalFileNameWithoutExtension + fileCount.ToString() + extension;
                    filePath = Path.Combine(folder, fileName);
                    blobClient = containerClient.GetBlobClient(filePath);
                    fileCount++;
                }
            }

            await blobClient.UploadAsync(fileStream);

            return filePath;
        }
    }
}
