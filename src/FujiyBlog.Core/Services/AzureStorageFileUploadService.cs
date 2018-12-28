using FujiyBlog.Core.EntityFramework;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Threading.Tasks;

namespace FujiyBlog.Core.Services
{
    public class AzureStorageFileUploadService : IFileUploadService
    {
        private readonly string accountName;
        private readonly string accountKey;
        private readonly string uploadContainerName;

        public AzureStorageFileUploadService(SettingRepository settings)
        {
            accountName = settings.AzureStorageAccountName;
            accountKey = settings.AzureStorageAccountKey;
            uploadContainerName = settings.AzureStorageUploadContainerName;
        }

        public async Task<string> UploadFile(Stream fileStream, string filePath)
        {
            StorageCredentials storageCredentials = new StorageCredentials(accountName, accountKey);
            CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference(uploadContainerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(filePath);

            if (await blockBlob.ExistsAsync())
            {
                string folder = Path.GetDirectoryName(filePath);
                string originalFileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                string extension = Path.GetExtension(filePath);

                int fileCount = 2;
                while (await blockBlob.ExistsAsync())
                {
                    string fileName = originalFileNameWithoutExtension + fileCount.ToString() + extension;
                    filePath = Path.Combine(folder, fileName);
                    blockBlob = container.GetBlockBlobReference(filePath);
                    fileCount++;
                }
            }

            await blockBlob.UploadFromStreamAsync(fileStream);

            return filePath;
        }
    }
}
