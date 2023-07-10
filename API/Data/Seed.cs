using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager, 
                                           RoleManager<AppRole> roleManager)
        {
            if(await userManager.Users.AnyAsync()) return;

            var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");

            var options = new JsonSerializerOptions{PropertyNameCaseInsensitive = true};

            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            var roles = new List<AppRole>
            {
                new AppRole{Name="User"},
                new AppRole{Name="Admin"},
                new AppRole{Name="Moderator"},
            };

            foreach(var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            foreach(var user in users)
            {
               
                user.UserName = user.UserName.ToLower();               
                await userManager.CreateAsync(user,"Password@123");
                await userManager.AddToRoleAsync(user,"User");
            }

            var admin = new AppUser{
                UserName="admin"
            };
            await userManager.CreateAsync(admin,"Password@123");
            await userManager.AddToRolesAsync(admin, new[] {"Admin","Moderator"} );

            
        }
    }
}