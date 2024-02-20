using System.IO;
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
    }
}
