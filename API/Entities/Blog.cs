namespace API.Entities
{
    public class Blog
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string PublicId { get; set; }
        public List<Post> Posts { get; set; } = new();
        public int AppUserId { get; set; }
        public AppUser Appuser { get; set; }
    }
}