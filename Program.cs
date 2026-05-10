using Microsoft.EntityFrameworkCore;
using SatoraCaffeRestaurantTracking.Models;
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);

// --- 1. SERVÝS AYARLARI ---


builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // Türkçe karakterlerin (Unicode) bozulmamasýný saðlar
        options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
    });

builder.Services.AddDbContext<CafeContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SatoraContext")));


// SESSION AYARLARI 
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturum 30 dk açýk kalsýn
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // tarayýcý oturumu kaydeder
});

var app = builder.Build();

// --- 2. MIDDLEWARE HATTI ---
//gelen isteðin hangi yollardan geçeceðini belirler

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

// Rota Ayarlarý
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}"); // Açýlýþ sayfasý: Login

app.Run();