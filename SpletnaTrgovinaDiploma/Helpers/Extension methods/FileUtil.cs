using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace SpletnaTrgovinaDiploma
{
    public static class FileUtil
    {
        static string GetRootWebDirectoryName(IHostEnvironment hostEnvironment)
            => hostEnvironment.ContentRootPath + "/wwwroot";

        public static string UploadImageFile(this IFormFile imageFile, IHostEnvironment hostEnvironment)
        {
            const string relativePath = "/uploadedFiles/";

            if (imageFile == null)
                return null;

            var fileInfo = new FileInfo(imageFile.FileName);
            var fileName = Path.GetRandomFileName() + fileInfo.Extension;
            var absolutePath = GetRootWebDirectoryName(hostEnvironment) + relativePath;
            var fileNameWithPath = absolutePath + fileName;

            if (!Directory.Exists(absolutePath))
                Directory.CreateDirectory(absolutePath);

            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                imageFile.CopyTo(stream);
            }

            return relativePath + fileName;
        }

        public static async Task<string> DownloadImageAndStoreIt(string imageUrl, IHostEnvironment hostEnvironment)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return "";

            var extension = imageUrl.Substring(imageUrl.Length - 3);

            using var client = new HttpClient();
            var imageByteArray = await client.GetByteArrayAsync(imageUrl);
            var memoryStream = new MemoryStream(imageByteArray);
            var imageFile = new FormFile(memoryStream, 0, memoryStream.Length, "streamImage", $"streamImage.{extension}");

            return imageFile.UploadImageFile(hostEnvironment);
        }
    }
}
