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

            try
            {
                using (var image = Image.ThumbnailStream(imageFile.OpenReadStream(), 400, crop: Enums.Interesting.Attention))
                {
                    image.WriteToFile(fileNameWithPath);
                }
            }
            catch (VipsException)
            {

            }

            return relativePath + fileName;
        }

        public static async Task<string> DownloadImageAndStoreIt(string imageUrl, IHostEnvironment hostEnvironment)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return "";

            using var client = new HttpClient();
            var response = await client.GetAsync(imageUrl);
            var mediaType = response.Content.Headers.ContentType?.MediaType ?? "";
            var splittedMediaType = mediaType.Split('/');
            var extension = splittedMediaType.Length > 1 ? splittedMediaType[1] : "jpg";

            var imageByteArray = await response.Content.ReadAsByteArrayAsync();
            var memoryStream = new MemoryStream(imageByteArray);
            var imageFile = new FormFile(
                memoryStream,
                0,
                memoryStream.Length,
                "streamImage",
                $"streamImage.{extension}");

            return imageFile.UploadImageFile(hostEnvironment);
        }
    }
}
