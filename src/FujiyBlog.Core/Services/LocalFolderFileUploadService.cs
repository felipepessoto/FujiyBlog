using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;

namespace FujiyBlog.Core.Services
{
    public class LocalFolderFileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment env;

        public LocalFolderFileUploadService(IWebHostEnvironment env)
        {
            this.env = env;
        }

        public async Task<string> UploadFile(Stream stream, string filePath)
        {
            string baseFolder = Path.Combine(env.WebRootPath, "Upload");
            filePath = Path.Combine(baseFolder, filePath);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            if (File.Exists(filePath))
            {
                string folder = Path.GetDirectoryName(filePath);
                string originalFileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                string extension = Path.GetExtension(filePath);
    
                int fileCount = 2;
                while (File.Exists(filePath))
                {
                    string fileName = originalFileNameWithoutExtension + fileCount.ToString() + extension;
                    filePath = Path.Combine(folder, fileName);
                    fileCount++;
                }
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await stream.CopyToAsync(fileStream);
            }
            return filePath.Substring(baseFolder.Length + 1);
        }
    }
}
