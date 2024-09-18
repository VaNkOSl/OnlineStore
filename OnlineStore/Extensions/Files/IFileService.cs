namespace OnlineStore.Extensions.Files;

public interface IFileService
{
    Task<string> UploadFileAsync(IFormFile file, IWebHostEnvironment webHostEnvironment, string folderName = "images");
}
