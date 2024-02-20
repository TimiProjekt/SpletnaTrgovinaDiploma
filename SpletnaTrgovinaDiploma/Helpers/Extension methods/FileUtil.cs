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
            if (imageFile == null)
                return null;

            var fileInfo = new FileInfo(imageFile.FileName);
            var relativePath = "/uploadedFiles/" + Path.GetRandomFileName() + fileInfo.Extension;
            var fileNameWithPath = GetRootWebDirectoryName(hostEnvironment) + relativePath;

            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                imageFile.CopyTo(stream);
            }

            return relativePath;
        }
    }
}
