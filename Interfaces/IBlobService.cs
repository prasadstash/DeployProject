namespace DeployProject.Interfaces
{
    public interface IBlobService
    {
        Task<string> UploadFileAsync(IFormFile file, string fileName);
    }
}
