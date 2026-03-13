using Gallery.BusinessLogic.Services;
using Gallery.DataAccess;
using Gallery.DataAccess.Repositories.ImageRepositories;
using Gallery.DataAccess.Repositories.UserRepositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("GalleryConnection");


builder.Services.AddDbContext<GalleryContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();


builder.Services.AddScoped<ImageService>();
builder.Services.AddScoped<UserService>();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
