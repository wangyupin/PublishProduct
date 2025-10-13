using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HqSrv.Infrastructure.Helpers
{
    public static class ImageFileHelper
    {
        /// <summary>
        /// 從檔案路徑讀取圖片並轉成 IFormFile
        /// </summary>
        /// <param name="relativePath">相對路徑,例如: BackendImages/PublishGoods/xxx/xxx_B1.jpg</param>
        /// <param name="hostEnvironment">IWebHostEnvironment 實例</param>
        /// <returns>IFormFile 或 null</returns>
        public static async Task<IFormFile?> ReadImageFromPath(string relativePath, IWebHostEnvironment hostEnvironment)
        {
            if (string.IsNullOrEmpty(relativePath))
                return null;

            try
            {
                string fullPath = Path.Combine(hostEnvironment.ContentRootPath, relativePath);

                if (!File.Exists(fullPath))
                    return null;

                byte[] fileBytes = await File.ReadAllBytesAsync(fullPath);
                var memoryStream = new MemoryStream(fileBytes);
                string fileName = Path.GetFileName(fullPath);

                var formFile = new FormFile(memoryStream, 0, fileBytes.Length, "file", fileName)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = GetContentType(fileName)
                };

                return formFile;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 根據檔案副檔名取得 Content-Type
        /// </summary>
        private static string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }
    }
}