using System;
namespace G3.Dtos
{
	public class MilestoneDto
	{
		[Required]
		public string Title { get; set; } = null!;
		public string? Description { get; set; }
		[Required]
		public string StartDate { get; set; } = null!;
        [Required]
        public string DueDate { get; set; } = null!;
    }
}

