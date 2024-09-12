using BlogProjectPrac7.Services.Interfaces;

namespace BlogProjectPrac7.Services
{
    public class ImageService : IImageService
    {
        public string ContentType(IFormFile file)
        {
            return file?.ContentType!;
        }

        public string DecodeImage(byte[] data, string type)
        {
            //Takes an image out of the db.
            if (data is null || type is null) return null;
            return $"data:image/{type};base64,{Convert.ToBase64String(data)}";
        }

        public async Task<byte[]> EncodeImageAsync(IFormFile file)
        {
            //Going into the db
            if (file is null)
            {
                return null!;
            }
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            return ms.ToArray();
        }

        public async Task<byte[]> EncodeImageAsync(string fileName)
        {
            var file = $"{Directory.GetCurrentDirectory()}/wwwroot/img/{fileName}";
            return await File.ReadAllBytesAsync(file);
        }

        public int Size(IFormFile file)
        {
            return Convert.ToInt32(file?.Length);
        }
    }
}
