using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace LiveSupport.AI.Models
{
    public class UserConnection
    {
        [Required]
        public string  Name { get; set; } = string.Empty;
 
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public bool IsAdmin { get; set; } = false;

        public int SiteId { get; set; } = 0;

        
    }
}
