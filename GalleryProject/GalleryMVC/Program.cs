using Gallery.BusinessLogic.Services;
using Gallery.BusinessLogic.Services.ImageServices;
using Gallery.BusinessLogic.Services.UserService;
using Gallery.DataAccess;
using Gallery.DataAccess.Repositories.ImageRepositories;
using Gallery.DataAccess.Repositories.UserRepositories;
using GalleryMVC.Filters;
using GalleryMVC.Middleware;
using GalleryMVC.SignalR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("GalleryConnection");

builder.Services.AddDbContext<GalleryContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IUserService, UserService>();


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;          
    options.Cookie.IsEssential = true;            
});

builder.Services.AddSignalR();

builder.Services.AddScoped<ControllerLoggingFilter>();
// Add global exception filter
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});

var app = builder.Build();

// use Middleware
app.UseMiddleware<RequestDurationMiddleware>();


// map Middleware
app.Map("/gallery-info", infoApp =>
{
    Console.WriteLine("== [MAP BRANCH] Pipeline branched to /gallery-info");
    // run Middleware
    infoApp.Run(async context =>
    {
        Console.WriteLine("== [TERMINAL RUN] Executing terminal handler inside branch.");
        var response = $@"
            <html>
            <body style='font-family: sans-serif; padding: 20px;'>
                <h1 style='color: #2c3e50;'>Gallery Project Status</h1>
                <hr/>
                <p><b>Environment:</b> {app.Environment.EnvironmentName}</p>
                <p><b>System Date:</b> {DateTime.Now:f}</p>
                <p style='color: green;'><b>Status:</b> All systems operational</p>
                <a href='/'>Return to Gallery</a>
            </body>
            </html>";

        context.Response.ContentType = "text/html";
        await context.Response.WriteAsync(response);
    });
});

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

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Image}/{action=Index}/{id?}");

app.MapHub<GalleryHub>("/galleryHub");

app.Run();
