using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Common
{
    public class FileStorageService : IStorageService
    {
        private readonly string _userContentFolder;
        private const string USER_CONTENT_FODER_NAME = "user-content";
        public FileStorageService(IWebHostEnvironment webHostEnviroment)
        {
            _userContentFolder = Path.Combine(webHostEnviroment.WebRootPath, USER_CONTENT_FODER_NAME);

        }
        public Task DeleteFileAsync(string fileName)
        {
            throw new NotImplementedException();
        }

        public string GetFileUrl(string fileName)
        {
            return $"/{USER_CONTENT_FODER_NAME}/{fileName}";
        }

        public async Task SaveFileAsync(Stream mediaBinaryStream, string fileName)
        {
            var filePath = Path.Combine(_userContentFolder, fileName);
            using var output = new FileStream(filePath, FileMode.Create);
            await mediaBinaryStream.CopyToAsync(output);
        }
    }
}
