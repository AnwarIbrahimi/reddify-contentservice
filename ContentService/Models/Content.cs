using System.ComponentModel.DataAnnotations;

namespace ContentService.Models
{
    public class Content
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
