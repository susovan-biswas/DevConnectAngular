namespace API.Entities
{
    public class Messages
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderUsername { get; set; }
        public AppUser Sender { get; set; }
        public int RecipientId { get; set; }
        public string RecipientUsername { get; set; }
        public AppUser Recipient { get; set; }

        public string MessageBody { get; set; }

        public DateTime? DateRead { get; set; }

        public DateTime MessageSentAt { get; set; } = DateTime.UtcNow;

        public bool SenderDeleted { get; set; }
        public bool RecipientDeleted { get; set; }
    }
}