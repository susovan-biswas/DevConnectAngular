namespace API.DTOs
{
    public class CreateMessageDto
    {
        public string Recipient { get; set; }
        public string MessageBody { get; set; }
    }
}