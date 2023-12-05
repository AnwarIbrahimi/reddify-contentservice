using System.ComponentModel.DataAnnotations;

namespace ContentService.DTO
{
    public class ContentReadDTO
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Publisher { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
