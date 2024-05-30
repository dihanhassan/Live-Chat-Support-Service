using System.ComponentModel.DataAnnotations;

namespace LiveSupport.AI.Models
{
    public class PrivateMessage
    {
        [Required]
        public string Message { get; set; } = string.Empty;
        [Required]
        public string RoomID { get; set; } = string.Empty;
    }
}
