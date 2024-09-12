namespace BlogProjectPrac7.Services.Interfaces
{
    public interface IImageService
    {
        //Uploading the image
        Task<byte[]> EncodeImageAsync(IFormFile file);
        //Uploading an Image that is a static image/Path.
        Task<byte[]> EncodeImageAsync(string fileName);
        //Unload the image for display purposes
        string DecodeImage(byte[] data, string type);
        //returns the content type of the image .jpeg/.Png
        string ContentType(IFormFile file);
        int Size(IFormFile file);
    }
}
