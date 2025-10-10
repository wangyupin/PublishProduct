using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using System.Collections;
using System.Net.Http;
using POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods;
using System.IO.Compression;
using System.Text;

namespace POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.Common
{
    public class ResizeRule
    {
        public int MinWidth { get; set; } = 0;  // 最小寬度
        public int MinHeight { get; set; } = 0; // 最小高度
        public int FixedWidth { get; set; } = 0;  // 固定寬度 (0表示不使用固定寬度)
        public int FixedHeight { get; set; } = 0; // 固定高度 (0表示不使用固定高度)
        public bool MaintainAspectRatio { get; set; } = true; // 是否保持原始比例
        public long TargetSizeInBytes { get; set; } = 0; // 目標檔案大小
    }

    public class ImageCompressor
    {
        public static IFormFile CompressAndResizeImage(IFormFile file, long targetSizeInBytes, int minWidth, int minHeight)
        {
            using (var stream = file.OpenReadStream())
            using (Bitmap bmp = new Bitmap(stream))
            {
                if (bmp.Width < minWidth || bmp.Height < minHeight)
                {
                    throw new ArgumentException($"圖片解析度不可低於 {minWidth}x{minHeight}.");
                }

                Bitmap resizedBmp = ResizeImage(bmp, minWidth, minHeight);
                byte[] compressedBytes = CompressImageToTargetSize(resizedBmp, targetSizeInBytes);

                var compressedStream = new MemoryStream(compressedBytes);
                var formFile = new FormFile(compressedStream, 0, compressedStream.Length, file.Name, file.FileName)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = file.ContentType
                };

                return formFile;

            }
        }

        public static IFormFile CompressAndResizeImageToFixedSize(IFormFile file, long targetSizeInBytes, int fixedWidth, int fixedHeight, bool maintainAspectRatio = true)
        {
            using (var stream = file.OpenReadStream())
            using (Bitmap bmp = new Bitmap(stream))
            {
                int newWidth, newHeight;

                if (maintainAspectRatio)
                {
                    // 計算保持比例的尺寸
                    double ratioX = (double)fixedWidth / bmp.Width;
                    double ratioY = (double)fixedHeight / bmp.Height;
                    double ratio = Math.Min(ratioX, ratioY);

                    newWidth = (int)(bmp.Width * ratio);
                    newHeight = (int)(bmp.Height * ratio);
                }
                else
                {
                    // 直接使用固定尺寸
                    newWidth = fixedWidth;
                    newHeight = fixedHeight;
                }

                using (Bitmap resizedBmp = new Bitmap(newWidth, newHeight))
                {
                    using (Graphics g = Graphics.FromImage(resizedBmp))
                    {
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                        // 清空背景（白色）
                        g.Clear(Color.White);
                        g.DrawImage(bmp, 0, 0, newWidth, newHeight);
                    }

                    // 壓縮到目標大小
                    byte[] compressedBytes = CompressImageToTargetSize(resizedBmp, targetSizeInBytes);

                    // 創建 IFormFile
                    var compressedStream = new MemoryStream(compressedBytes);
                    var formFile = new FormFile(compressedStream, 0, compressedStream.Length, file.Name, file.FileName)
                    {
                        Headers = new HeaderDictionary(),
                        ContentType = file.ContentType
                    };

                    return formFile;
                }
            }
        }

        private static byte[] CompressImageToTargetSize(Bitmap bmp, long targetSizeInBytes)
        {
            long quality = 100;
            byte[] compressedBytes;

            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
            System.Drawing.Imaging.Encoder qualityEncoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameters encoderParameters = new EncoderParameters(1);

            using (var ms = new MemoryStream())
            {
                while (true)
                {
                    ms.SetLength(0);

                    EncoderParameter encoderParameter = new EncoderParameter(qualityEncoder, quality);
                    encoderParameters.Param[0] = encoderParameter;

                    bmp.Save(ms, jpgEncoder, encoderParameters);

                    if (ms.Length <= targetSizeInBytes || quality <= 0)
                    {
                        break;
                    }

                    quality -= 10;
                }

                compressedBytes = ms.ToArray();
            }

            return compressedBytes;
        }

        private static Bitmap ResizeImage(Bitmap bmp, int minWidth, int minHeight)
        {
            if (bmp.Width < minWidth || bmp.Height < minHeight)
            {
                int newWidth = bmp.Width < minWidth ? minWidth : bmp.Width;
                int newHeight = bmp.Height < minHeight ? minHeight : bmp.Height;

                var resizedBmp = new Bitmap(bmp, new Size(newWidth, newHeight));
                return resizedBmp;
            }

            return bmp;
        }
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }

    public static class ImageCompressorExtensions
    {
        public static bool IsBase64OrBlobImage(string src)
        {
            return !string.IsNullOrEmpty(src) &&
                   (src.StartsWith("data:image") || src.StartsWith("blob:"));
        }

        public static IFormFile ConvertBase64ToFormFile(string base64String)
        {
            // 移除Base64前綴
            var base64Data = Regex.Replace(base64String, @"^data:image\/[a-zA-Z]+;base64,", string.Empty);
            // 解碼Base64
            byte[] fileBytes = Convert.FromBase64String(base64Data);
            // 創建MemoryStream (移除 using 关键字)
            var stream = new MemoryStream(fileBytes);
            // 創建IFormFile
            return new FormFile(stream, 0, fileBytes.Length, "file", "image.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };
        }

        public static bool IsExternalImage(string src)
        {
            return !string.IsNullOrEmpty(src) &&
                   src.StartsWith("http") &&
                   !IsLocalImage(src);
        }

        private static bool IsLocalImage(string src)
        {
            // 這裡可以根據你的具體域名邏輯實現
            return src.Contains("yourdomain.com");
        }

        public static async Task<IFormFile> DownloadExternalImageAsFormFileAsync(string imageUrl)
        {
            using var httpClient = new HttpClient();
            var fileBytes = await httpClient.GetByteArrayAsync(imageUrl);

            var stream = new MemoryStream(fileBytes); 

            return new FormFile(stream, 0, fileBytes.Length, "file", Path.GetFileName(imageUrl))
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };
        }

        public static string CompressFolderToBase64(string folderPath, string[] allowedExtensions = null)
        {
            // 如果未指定副檔名，使用預設圖片副檔名
            if (allowedExtensions == null || !allowedExtensions.Any())
            {
                allowedExtensions = new[] { ".jpg", ".jpeg" };
            }

            // 取得資料夾中的圖片檔案
            var imageFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories)
                .Where(file => allowedExtensions.Contains(Path.GetExtension(file).ToLower()))
                .ToList();

            if (!imageFiles.Any())
            {
                throw new FileNotFoundException("沒有在指定資料夾中找到符合條件的圖片檔案");
            }

            // 創建臨時zip檔案
            string tempZipPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".zip");

            try
            {
                // 壓縮圖片檔案
                using (FileStream zipToOpen = new FileStream(tempZipPath, FileMode.Create))
                {
                    using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
                    {
                        foreach (string imagePath in imageFiles)
                        {
                            // 獲取相對路徑，保留資料夾結構
                            string relativePath = imagePath.Substring(folderPath.Length)
                                .TrimStart(Path.DirectorySeparatorChar);

                            // 添加檔案到zip
                            ZipArchiveEntry entry = archive.CreateEntry(relativePath, CompressionLevel.Optimal);

                            // 寫入檔案內容
                            using (Stream entryStream = entry.Open())
                            using (FileStream fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                            {
                                fileStream.CopyTo(entryStream);
                            }
                        }
                    }
                }

                // 將zip檔案轉換為Base64
                byte[] zipBytes = File.ReadAllBytes(tempZipPath);
                return Convert.ToBase64String(zipBytes);
            }
            finally
            {
                // 刪除臨時zip檔案
                if (File.Exists(tempZipPath))
                {
                    File.Delete(tempZipPath);
                }
            }
        }

        public static string CompressFolderToBase64WithPatternRules(string folderPath, Dictionary<string, ResizeRule> patternRules)
        {
            // 只處理jpg和jpeg檔案
            string[] allowedExtensions = new[] { ".jpg", ".jpeg" };

            // 取得資料夾中的圖片檔案
            var imageFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories)
                .Where(file => allowedExtensions.Contains(Path.GetExtension(file).ToLower()))
                .ToList();

            if (!imageFiles.Any())
            {
                throw new FileNotFoundException("沒有在指定資料夾中找到JPG/JPEG圖片檔案");
            }

            // 創建臨時zip檔案
            string tempZipPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".zip");

            // 創建臨時目錄來存放調整大小的圖片
            string tempResizedDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempResizedDir);

            try
            {
                // 處理每個圖片
                foreach (string imagePath in imageFiles)
                {
                    string fileName = Path.GetFileName(imagePath);
                    string relativePath = imagePath.Substring(folderPath.Length)
                        .TrimStart(Path.DirectorySeparatorChar);
                    string resizedImagePath = Path.Combine(tempResizedDir, relativePath);

                    // 確保目錄存在
                    Directory.CreateDirectory(Path.GetDirectoryName(resizedImagePath));

                    // 根據檔名找到匹配的調整規則
                    ResizeRule rule = GetResizeRuleForFile(fileName, patternRules);

                    // 將檔案轉換為 IFormFile
                    using (FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                    {
                        var fileLength = fs.Length;
                        var formFile = new FormFile(fs, 0, fileLength, "file", fileName)
                        {
                            Headers = new HeaderDictionary(),
                            ContentType = "image/jpeg"
                        };

                        try
                        {
                            IFormFile compressedFile = null;
                            // 直接使用 CompressAndResizeImage 處理圖片
                            if (rule.FixedWidth > 0 && rule.FixedHeight > 0)
                            {
  
                                compressedFile = ImageCompressor.CompressAndResizeImageToFixedSize(
                                    formFile,
                                    rule.TargetSizeInBytes,
                                    rule.FixedWidth,
                                    rule.FixedHeight,
                                    rule.MaintainAspectRatio);
                            }
                            else
                            {
                                compressedFile = ImageCompressor.CompressAndResizeImage(
                                formFile,
                                rule.TargetSizeInBytes,
                                rule.MinWidth,
                                rule.MinHeight);

                            }
                            

                            // 保存處理後的圖片
                            using (var fileStream = new FileStream(resizedImagePath, FileMode.Create))
                            {
                                compressedFile.OpenReadStream().CopyTo(fileStream);
                            }
                        }
                        catch (ArgumentException ex)
                        {
                            // 如果解析度不足，記錄警告並使用原始圖片
                            Console.WriteLine($"警告: {ex.Message} 將使用原始圖片。");
                            File.Copy(imagePath, resizedImagePath, true);
                        }
                    }
                }

                // 壓縮調整大小後的圖片
                using (FileStream zipToOpen = new FileStream(tempZipPath, FileMode.Create))
                {
                    using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create, true, Encoding.UTF8))
                    {
                        string[] resizedFiles = Directory.GetFiles(tempResizedDir, "*.*", SearchOption.AllDirectories);

                        foreach (string imagePath in resizedFiles)
                        {
                            // 獲取相對路徑，保留資料夾結構
                            string relativePath = imagePath.Substring(tempResizedDir.Length)
                                .TrimStart(Path.DirectorySeparatorChar);

                            // 添加檔案到zip
                            ZipArchiveEntry entry = archive.CreateEntry(relativePath, CompressionLevel.Optimal);

                            // 寫入檔案內容
                            using (Stream entryStream = entry.Open())
                            using (FileStream fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                            {
                                fileStream.CopyTo(entryStream);
                            }
                        }
                    }
                }

                // 將zip檔案轉換為Base64
                byte[] zipBytes = File.ReadAllBytes(tempZipPath);
                return Convert.ToBase64String(zipBytes);
            }
            finally
            {
                // 清理臨時檔案
                if (File.Exists(tempZipPath))
                {
                    File.Delete(tempZipPath);
                }

                // 清理臨時目錄
                if (Directory.Exists(tempResizedDir))
                {
                    Directory.Delete(tempResizedDir, true);
                }
            }
        }

        // 根據檔名取得適用的調整規則
        private static ResizeRule GetResizeRuleForFile(string fileName, Dictionary<string, ResizeRule> patternRules)
        {
            // 檢查每個模式是否匹配檔名
            foreach (var pattern in patternRules.Keys)
            {
                if (Regex.IsMatch(fileName, pattern, RegexOptions.IgnoreCase))
                {
                    return patternRules[pattern];
                }
            }

            // 如果沒有匹配的規則，返回預設規則
            return new ResizeRule
            {
                MinWidth = 800,
                MinHeight = 600,
                TargetSizeInBytes = 500 * 1024  // 預設 500KB
            };
        }

    }
}
