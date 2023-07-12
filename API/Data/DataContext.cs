using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace API.Data
{
    public class DataContext : IdentityDbContext<AppUser, AppRole, int, 
                               IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
                               IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }

        //public DbSet<AppUser> Users  { get; set; }
        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Messages> Messages { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Connections> Connections { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(u => u.User)
            .HasForeignKey(ur => ur.UserId)
            .IsRequired();

            builder.Entity<AppRole>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(u => u.Role)
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired();

            builder.Entity<UserLike>()
            .HasKey(k=> new {k.SourceUserId, k.TargetUserId});

            builder.Entity<UserLike>()
            .HasOne(s =>s.SourceUser)
            .WithMany(l =>l.LikedUsers)
            .HasForeignKey(s =>s.SourceUserId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserLike>()
            .HasOne(s =>s.TargetUser)
            .WithMany(l =>l.LikedByUsers)
            .HasForeignKey(s =>s.TargetUserId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Messages>()
            .HasOne(r => r.Recipient)
            .WithMany(m => m.ReceivedMessages)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Messages>()
            .HasOne(s=>s.Sender)
            .WithMany(m => m.SentMessages)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}