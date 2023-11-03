using System.ComponentModel.DataAnnotations;

namespace ContentService.DTO
{
    public class ContentReadDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Publisher { get; set; }
        public string Description { get; set; }
    }
}
