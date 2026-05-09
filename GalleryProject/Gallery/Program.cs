using Gallery.BusinessLogic.Services.ImageServices;
using Gallery.BusinessLogic.Services.UserService;
using Gallery.DataAccess;
using Gallery.DataAccess.Models;
using Gallery.DataAccess.Repositories.ImageRepositories;
using Gallery.DataAccess.Repositories.UserRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("GalleryConnection");


builder.Services.AddDbContext<GalleryContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<User>(options =>
{
    options.Password.RequireDigit = true;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddRoles<IdentityRole<int>>()
.AddEntityFrameworkStores<GalleryContext>();


builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();


builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

app.Run();
