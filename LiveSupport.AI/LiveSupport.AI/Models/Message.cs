namespace LiveSupport.AI.Models
{
    public class  Message
    {
        public string User { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime Time { get; set; } = DateTime.Now;
        
        
        public string Email { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;
    }
}
