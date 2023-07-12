using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace API.Entities
{
  
    public class Connections
    {
        public Connections()
        {
            
        }
        public Connections(string connectionId, string username)
        {
            ConnectionId = connectionId;
            Username = username;
        }

        [Key]
        public string ConnectionId { get; set; }
        public string Username { get; set; }
    }
}