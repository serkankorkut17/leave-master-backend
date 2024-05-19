using leave_master_backend.Context;
using Microsoft.EntityFrameworkCore;
using leave_master_backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//** MongoDB Connection **//
builder.Services.AddDbContext<MongoDBContext>(options => options.UseMongoDB("mongodb+srv://serkankorkut17:Merhaba123@cluster0.xz591i3.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0", "LeaveMasterDB"));

builder.Services.AddIdentity<AppUser, IdentityRole>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;

}).AddEntityFrameworkStores<MongoDBContext>();


builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = 
    options.DefaultChallengeScheme = 
    options.DefaultForbidScheme =
    options.DefaultScheme =
    options.DefaultSignInScheme = 
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"] ?? ""))
    };
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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

// app.MapPost("/api/user", async (MongoDBContext dbContext, User user) =>
// {   
//     // user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
//     await dbContext.Users.AddAsync(user);
//     await dbContext.SaveChangesAsync();
//     return user;
// });

// app.MapGet("/api/user", async (MongoDBContext dbContext) =>
// {
//     var users = await dbContext.Users.ToListAsync();
//     return Results.Ok(users);
// });

app.Run();

