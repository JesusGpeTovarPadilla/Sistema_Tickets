using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Sistema_Tickets.Data;
using System.Diagnostics;
using Sistema_Tickets.Models;
using Microsoft.AspNetCore.Components.Forms;
using System;
using Microsoft.AspNetCore.RateLimiting;


namespace Sistema_Tickets.Controllers
{
    public class AuthController : Controller
    {
        private readonly TicketSystemDbContext _context;

        public AuthController(TicketSystemDbContext context)
        {
            _context = context;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EnableRateLimiting("LoginPolicy")]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Validación de entrada
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Todos los campos son obligatorios";
                return View();
            }

            System.Diagnostics.Debug.WriteLine($"Intentando login con: {email}");

            // Buscar usuario con Email en campo "Correo", Estado activo y NO bloqueado
            var user = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Correo == email
                                        && u.Estado == true
                                        && u.Esbloqueado == false);

            // Usuario bloqueado
            if (user.Esbloqueado)
            {
                System.Diagnostics.Debug.WriteLine($"🔒 Usuario bloqueado: {email}");
                ViewBag.Error = "Tu cuenta ha sido bloqueada. Contacta al administrador.";
                return View();
            }

            // Validación consolidada
            if (user == null || user.Rol == null)
            {
                System.Diagnostics.Debug.WriteLine($"🛑 Login fallido para: {email}");
                ViewBag.Error = "Credenciales inválidas o cuenta inactiva";
                return View();
            }
            if(!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                System.Diagnostics.Debug.WriteLine($"🛑 Login fallido para: {email} con contraseña: {password}");
                ViewBag.Error = "Contraseña incorrecta";
                return View();
            }

            // Actualizar última sesión 
            user.UltimaSeccion = DateTime.Now;
            await _context.SaveChangesAsync();

            // Crear claims
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.UsuarioId.ToString()), // ID usuario
        new Claim(ClaimTypes.Name, user.Nombre), // Nombre del usuario
        new Claim(ClaimTypes.Email, user.Correo), // Correo del usuario
        new Claim(ClaimTypes.Role, user.Rol.Nombre) //rol del usuario
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            System.Diagnostics.Debug.WriteLine($"✅ Login exitoso: {user.Nombre} ({user.Rol.Nombre})");

            // Redirección según rol
            return user.Rol.Nombre switch
            {
                "Administrador" => RedirectToAction("Index", "Admin"),
                "Gerente" => RedirectToAction("Index", "Gerente"),
                "Tecnico" => RedirectToAction("Index", "Tecnico"),
                "Usuario" => RedirectToAction("Index", "usuario"),
                _ => RedirectToAction("Denegado")
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult Denegado() => View();


        [HttpGet]
        public async Task<IActionResult> SeedUsuarios()
        {
            try
            {
                // Contraseña que usaremos para todos
                string passwordComun = "Admin123!";

                // Lista de usuarios a crear
                var usuariosIniciales = new List<UsuarioSeed>
        {
            new UsuarioSeed { Nombre = "Carlos Administrador", Correo = "admin@empresa.com", RolID = 1, DepartamentoID = 4 },
            new UsuarioSeed { Nombre = "María Gerente TI", Correo = "gerente.ti@empresa.com", RolID = 2, DepartamentoID = 1 },
            new UsuarioSeed { Nombre = "Juan Gerente Ventas", Correo = "gerente.ventas@empresa.com", RolID = 2, DepartamentoID = 2 },
            new UsuarioSeed { Nombre = "Ana Vendedora", Correo = "ana.ventas@empresa.com", RolID = 3, DepartamentoID = 2 },
            new UsuarioSeed { Nombre = "Pedro Vendedor", Correo = "pedro.ventas@empresa.com", RolID = 3, DepartamentoID = 2 },
            new UsuarioSeed { Nombre = "Luis Técnico", Correo = "luis.soporte@empresa.com", RolID = 4, DepartamentoID = 1 },
            new UsuarioSeed { Nombre = "Sofia Técnica", Correo = "sofia.soporte@empresa.com", RolID = 4, DepartamentoID = 1 }
        };

                var resultados = new List<string>();

                foreach (var userData in usuariosIniciales)
                {
                    // Guardar en variable local para evitar problemas con LINQ
                    string correoActual = userData.Correo;

                    // Verificar si ya existe
                    var existente = await _context.Usuarios
                        .FirstOrDefaultAsync(u => u.Correo == correoActual);

                    if (existente != null)
                    {
                        // Actualizar contraseña del existente
                        existente.PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordComun);
                        existente.Estado = true;
                        existente.Esbloqueado = false;
                        _context.Usuarios.Update(existente);
                        resultados.Add($"✏️ Actualizado: {userData.Correo}");
                    }
                    else
                    {
                        // Crear nuevo usuario
                        var nuevoUsuario = new Usuario
                        {
                            Nombre = userData.Nombre,
                            Correo = userData.Correo,
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordComun),
                            RolId = userData.RolID,
                            DepartamentoId = userData.DepartamentoID,
                            Estado = true,
                            Esbloqueado = false,
                            FechaAlta = DateTime.Now
                        };

                        _context.Usuarios.Add(nuevoUsuario);
                        resultados.Add($"✅ Creado: {userData.Correo}");
                    }
                }

                await _context.SaveChangesAsync();

                var mensaje = string.Join("<br>", resultados);
                return Content($@"
            <h2>Usuarios Iniciales Creados/Actualizados</h2>
            <p><strong>Contraseña para todos:</strong> {passwordComun}</p>
            <hr>
            {mensaje}
            <hr>
            <p><a href='/Auth/Login'>Ir al Login</a></p>
        ", "text/html");
            }
            catch (Exception ex)
            {
                return Content($"❌ Error: {ex.Message}<br><br>{ex.InnerException?.Message}");
            }
        }

        

    }

    // Clase auxiliar - agregar al final del archivo AuthController.cs (fuera de la clase)
    public class UsuarioSeed
    {
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public int RolID { get; set; }
        public int? DepartamentoID { get; set; }
    }
}
