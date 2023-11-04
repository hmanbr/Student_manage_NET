namespace G3.Services
{
	public class LocalFileUploadService : IFileUploadService
	{
		private readonly IWebHostEnvironment _environment;

        public LocalFileUploadService(IWebHostEnvironment environment)
        {
            this._environment = environment;
        }


		public async Task<string> UploadFileAsync(IFormFile file, string folderName)
		{
			var uploadsRoot = Path.Combine(_environment.ContentRootPath, "wwwroot", "Uploads", "Submits");
			var folderPath = Path.Combine(uploadsRoot, folderName);

			// Create the folder if it doesn't exist
			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}

			var currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
			var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
			var fileExtension = Path.GetExtension(file.FileName);
			var fileNameWithDate = $"{fileNameWithoutExtension}_{currentDate}{fileExtension}";
			var filePath = Path.Combine(folderPath, fileNameWithDate);

			using (var fileStream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(fileStream);
			}
			var relativeUrl = Path.Combine("Uploads", "Submits", folderName, fileNameWithDate).Replace("\\", "/");

			return relativeUrl;
		}

		/*public async Task<string> UploadFileAsync(IFormFile file, string folderName)
		{
			var uploadsRoot = Path.Combine(_environment.ContentRootPath, "wwwroot", "Uploads", "Submits");
			var folderPath = Path.Combine(uploadsRoot, folderName);

			// Create the folder if it doesn't exist
			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}

			var filePath = Path.Combine(folderPath, file.FileName);

			using (var fileStream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(fileStream);
			}

			var relativeUrl = Path.Combine("Uploads", "Submits", folderName, file.FileName).Replace("\\", "/");

			return relativeUrl;
		}*/


		/*public async Task<string> UploadFileAsync(IFormFile file)
		{
			var filePath = Path.Combine(_environment.ContentRootPath, @"wwwroot\Uploads\Submits", file.FileName);
			using var fileStream = new FileStream(filePath, FileMode.Create);
			await file.CopyToAsync(fileStream);

			return filePath;
		}*/
	}
}
