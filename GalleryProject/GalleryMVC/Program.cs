using Gallery.BusinessLogic.Services;
using Gallery.DataAccess;
using Gallery.DataAccess.Repositories.ImageRepositories;
using Gallery.DataAccess.Repositories.UserRepositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("GalleryConnection");


builder.Services.AddDbContext<GalleryContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();


builder.Services.AddScoped<ImageService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddControllersWithViews();

// Додаємо підтримку сесій
builder.Services.AddDistributedMemoryCache(); // Додає кеш у пам'яті для зберігання сесій

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Час життя сесії (наприклад, 30 хвилин)
    options.Cookie.HttpOnly = true;                // Захист від JS-скриптів (безпека)
    options.Cookie.IsEssential = true;             // Обов'язкова кука для роботи сайту
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Додаємо middleware для роботи з сесіями

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Image}/{action=Index}/{id?}");

app.Run();
