using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using FixMedicalWeb.Models;
using FixMedicalWeb.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. LEER LA CADENA DE CONEXIÓN
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. REGISTRAR EL CONTEXTO DE LA BASE DE DATOS
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString)
);

// 3. REGISTRAR EL SERVICIO DE HASHING (Antes de Build)
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

// 4. CONFIGURAR AUTENTICACIÓN POR COOKIES (Para que funcione el Login)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login"; // Ruta de tu Razor Page de login
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

// 5. AGREGAR SOPORTE PARA RAZOR PAGES
builder.Services.AddRazorPages();

var app = builder.Build();

/////

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Habilitar Autenticación y Autorización en el orden correcto
app.UseAuthentication();
app.UseAuthorization();

// Mapear tus páginas Razor (carpeta Pages)
app.MapRazorPages();

app.Run();