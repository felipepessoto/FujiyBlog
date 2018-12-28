using System.IO;
using System.Threading.Tasks;

namespace FujiyBlog.Core.Services
{
    public interface IFileUploadService
    {
        Task<string> UploadFile(Stream fileStream, string filePath);
    }
}