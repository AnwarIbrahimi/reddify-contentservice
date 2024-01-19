using System.ComponentModel.DataAnnotations;

namespace ContentService.DTO
{
    public class ContentCreateDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Publisher { get; set; }
        [Required]
        public string Description { get; set; }
        public string? Uid { get; set; }
    }
}
