using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using NetVips;

namespace SpletnaTrgovinaDiploma
{
    public static class ImageFileUtil
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

            //if extension == png then convert to jpg

            using (var image = Image.ThumbnailStream(imageFile.OpenReadStream(), 400, crop: Enums.Interesting.Attention))
            {
                image.WriteToFile(fileNameWithPath);
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
