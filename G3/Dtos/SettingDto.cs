using System;
namespace G3.Dtos
{
	public class RoleSettingDto
	{
		public RoleSettingDto()
		{
		}

        public string Type { get; set; } = "ROLE";

		[Required]
        public string Name { get; set; } = null!;
    }

    public class DomainSettingDto {
        public DomainSettingDto() {
        }

        public string Type { get; set; } = "DOMAIN";

        [Required]
        public string Name { get; set; } = null!;
    }
}

