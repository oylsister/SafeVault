using Microsoft.EntityFrameworkCore;
using SafeVault.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<SafeVaultContext>(options =>
{
    options.UseSqlite("Data Source=mydatabase.db");
});

builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/Auth/Login"; // Redirect to login page if not authenticated
        options.AccessDeniedPath = "/Auth/AccessDenied"; // Redirect if access is denied
    });

builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<SafeVaultContext>();

if (context.Users.Where(p => p.Username == "admin").FirstOrDefault() == null)
{
    context.Users.Add(new User
    {
        Username = "admin",
        Email = "admin@example.com",
        Password = BCrypt.Net.BCrypt.HashPassword("Admin123"),
        Role = "Admin"
    });

    await context.SaveChangesAsync();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
