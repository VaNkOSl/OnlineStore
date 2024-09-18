namespace OnlineStore.Extensions.Files;

public class FileService : IFileService
{
    public async Task<string> UploadFileAsync(IFormFile file, IWebHostEnvironment webHostEnvironment, string folderName = "images")
    {
       if(file != null && file.Length > 0)
       {
            var uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, folderName);

            if(!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFilesName = Guid.NewGuid().ToString() + "_" + file.Name;
            var filePath = Path.Combine(uploadsFolder, uniqueFilesName);

            using(var fileStream = new FileStream(filePath,FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return "/Product/images/" + uniqueFilesName;
       }
       else
       {
            throw new InvalidOperationException("Plese select an image file");
       }
    }
}
