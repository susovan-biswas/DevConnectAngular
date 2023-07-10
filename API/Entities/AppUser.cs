using System.Collections.Concurrent;
using API.Extensions;
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    public class AppUser:IdentityUser<int>
    {
        //public int Id { get; set; }
        //public string UserName { get; set; }
        //public byte[] PasswordHash { get; set; }
        //public byte[] PasswordSalt { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        public string Introduction { get; set; }
        public string FullName { get; set; }
        public string Skills { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public List<Photo> Photos { get; set; } = new();
        public List<Blog> Blogs { get; set; } = new();
        public List<UserLike> LikedByUsers { get; set; }
        public List<UserLike> LikedUsers { get; set; }
        public List<Messages> SentMessages { get; set; }
        public List<Messages> ReceivedMessages { get; set; }

        public ICollection<AppUserRole> UserRoles { get; set; }
    }
} 