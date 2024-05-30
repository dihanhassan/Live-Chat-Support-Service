namespace LiveSupport.AI.Data
{
    public static class SD
    {
        public static Dictionary<int, List<string>> _siteAdmin;

        static SD()
        {
            _siteAdmin = new Dictionary<int, List<string>>
            {
                { 1, new List<string> { "admin@gmail.com", "admin2@gmail.com" } },
                { 2, new List<string> { "abc@admin2@gmail.com", "admin4@gmail.com" } },
            };
        }
    }
}
