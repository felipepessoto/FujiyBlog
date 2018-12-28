using FujiyBlog.Core.EntityFramework;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FujiyBlog.Core.Services
{
    public static class FileUploadServiceFactory
    {
        public static IFileUploadService Build(IServiceProvider serviceProvider)
        {
            var settings = serviceProvider.GetService<SettingRepository>();
            var serviceName = settings.FileUploadService;

            switch (serviceName)
            {
                case nameof(AzureStorageFileUploadService):
                    return serviceProvider.GetService<AzureStorageFileUploadService>();
                case nameof(LocalFolderFileUploadService):
                    return serviceProvider.GetService<LocalFolderFileUploadService>();
                default:
                    throw new InvalidOperationException($"Can't find IFileUploadService named {serviceName}");
            }            
        }
    }
}
