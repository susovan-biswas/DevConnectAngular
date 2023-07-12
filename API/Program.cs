using API.Data;
using API.Entities;
using API.Extensions;
using API.Middleware;
using API.Services;
using API.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);


var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(builder => builder.AllowAnyHeader()
                             .AllowAnyMethod()
                             .AllowCredentials()                             
                             .WithOrigins("https://localhost:4200"));
app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/message");

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetService<DataContext>();
    var userManager =services.GetRequiredService<UserManager<AppUser>>();
    var roleManager =services.GetRequiredService<RoleManager<AppRole>>();
    await context.Database.MigrateAsync();
    await context.Database.ExecuteSqlRawAsync("Delete from [Connections]");
    await Seed.SeedUsers(userManager, roleManager);
}
catch (Exception ex)
{    
   var logger = services.GetService<ILogger<Program>>();
   logger.LogError(ex, "An error occured during Migration");
}
app.Run();
