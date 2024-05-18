using leave_master_backend.Context;
using Microsoft.EntityFrameworkCore;
using leave_master_backend.Models;

var builder = WebApplication.CreateBuilder(args);

//** MongoDB Connection **//
builder.Services.AddDbContext<MongoDBContext>(options => options.UseMongoDB("mongodb+srv://serkankorkut17:Merhaba123@cluster0.xz591i3.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0", "LeaveMasterDB"));

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

app.MapPost("/api/user", async (MongoDBContext dbContext, User user) =>
{   
    // user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
    await dbContext.Users.AddAsync(user);
    await dbContext.SaveChangesAsync();
    return user;
});

app.MapGet("/api/user", async (MongoDBContext dbContext) =>
{
    var users = await dbContext.Users.ToListAsync();
    return Results.Ok(users);
});


app.MapControllers();

app.Run();

