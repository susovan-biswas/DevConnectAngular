namespace API.DTOs
{
    public class BlogDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public List<PostDto> Posts { get; set; } 
    }
}