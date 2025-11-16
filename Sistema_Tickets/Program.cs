using System;
using System.Threading.RateLimiting;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Sistema_Tickets.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Conexión a la base de datos
builder.Services.AddDbContext<TicketSystemDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//Se registra el DbContext (DbventaContext) como servicio.
//Usa la cadena de conexión que tienes definida en appsettings.json bajo el nombre "DefaultConnection".

//2, Autentificacion mediante cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";  // Ruta de tu página de login
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied"; // Opcional
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;  // Seguridad adicional
        options.Cookie.SameSite = SameSiteMode.Strict;  // Protección CSRF
        //options.Cookie.SecurePolicy = CookieSecurePolicy.Always;  // Solo HTTPS en producción
    });
builder.Services.AddAuthorization();


//3. configuramos Rate Limiting Nativo .NET 8
//version nativa de .net 8
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Configuración específica para login
    options.AddPolicy("LoginPolicy", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,  // 5 intentos
                Window = TimeSpan.FromMinutes(1),  // cada 1 minutos
                AutoReplenishment = true
            }));
});

builder.Services.AddControllersWithViews(); // Permite que ASP.NET Core reconozca y ejecute los controladores
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage(); 
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRateLimiter(); // agregar antes de UseRouting, 
app.UseRouting();
app.UseAuthentication(); // se agrega antes que authorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
