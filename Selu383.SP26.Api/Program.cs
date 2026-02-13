using Microsoft.EntityFrameworkCore;
using Selu383.SP26.Api.Data;
using Selu383.SP26.Api.Features.Locations;
using Selu383.SP26.Api.Features.Users;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DataContext")));

builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/api/authentication/login";
        options.LogoutPath = "/api/authentication/logout";
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
    db.Database.Migrate();

    // Seed roles
    if (!db.Roles.Any())
    {
        db.Roles.AddRange(
            new Role { Name = "Admin" },
            new Role { Name = "User" }
        );
        db.SaveChanges();
    }

    // Seed test users
    if (!db.Users.Any())
    {
        var adminRole = db.Roles.First(x => x.Name == "Admin");
        var userRole = db.Roles.First(x => x.Name == "User");

        var bob = new User
        {
            UserName = "bob",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            Roles = new List<Role> { userRole }
        };

        var galkadi = new User
        {
            UserName = "galkadi",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            Roles = new List<Role> { adminRole }
        };

        var sue = new User
        {
            UserName = "sue",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            Roles = new List<Role> { userRole }
        };

        db.Users.AddRange(bob, galkadi, sue);
        db.SaveChanges();
    }

    if (!db.Locations.Any())
    {
        db.Locations.AddRange(
            new Location { Name = "Location 1", Address = "123 Main St", TableCount = 10 },
            new Location { Name = "Location 2", Address = "456 Oak Ave", TableCount = 20 },
            new Location { Name = "Location 3", Address = "789 Pine Ln", TableCount = 15 }
        );
        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

//see: https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0
// Hi 383 - this is added so we can test our web project automatically
public partial class Program { }