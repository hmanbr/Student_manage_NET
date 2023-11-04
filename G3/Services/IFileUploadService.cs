using Microsoft.AspNetCore.Http;

namespace G3.Services
{
	public interface IFileUploadService
	{
		Task<string> UploadFileAsync(IFormFile file, string folderName);
	}
}
