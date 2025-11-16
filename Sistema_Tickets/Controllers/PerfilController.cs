using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_Tickets.Data;
using Sistema_Tickets.ViewModels.Perfil;

namespace Sistema_Tickets.Controllers
{
    public class PerfilController : Controller
    {
        private readonly TicketSystemDbContext _Context;
        private readonly IWebHostEnvironment _Environment;

        public PerfilController(TicketSystemDbContext context, IWebHostEnvironment environment)
        {
            _Context = context;
            _Environment = environment;
        }
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }
             var userId = int.Parse(userIdClaim);
            var usuario = await _Context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Departamento)
                .FirstOrDefaultAsync(u => u.UsuarioId == userId);
            if (usuario == null)
            {
                return NotFound();
            }
            var perfilVM = new ViewModels.Perfil.PerfilViewModel
            {
                PerfilID = usuario.UsuarioId,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                Imagen = usuario.Imagen,
                RolNombre = usuario.Rol.Nombre,
                PerfilDepartamento = usuario.Departamento != null ? usuario.Departamento.Nombre : "Sin Departamento",
                Estado = usuario.Estado,
                bloqueado = usuario.Esbloqueado,
                DateTime = usuario.FechaAlta ?? DateTime.Now,
                UltimaSesion = usuario.UltimaSeccion ?? DateTime.Now
            };

            return View(perfilVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditContraseña(CambiarContraseñaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["info"] = "Por favor corrija los errores en el formulario.";
                return RedirectToAction(nameof(Index));
            }

            var usuario = await _Context.Usuarios.FindAsync(model.PerfilID);
            if (usuario == null)
            {
                TempData["info"] = "Usuario no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            //verificar contraseña
            if (!BCrypt.Net.BCrypt.Verify(model.PasswordActual, usuario.PasswordHash))
            {
                TempData["Error"] = "La contraseña actual es incorrecta.";
                return RedirectToAction(nameof(Index));
            }
            try
            {
                
                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.PasswordNueva);
                await _Context.SaveChangesAsync();

                TempData["Success"] = "Contraseña cambiada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cambiar la contraseña: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditImagen(CambiarImagenViewModel model)
        {
            // numero 1: Validar el modelo
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Por favor corrija los errores en el formulario.";
                return RedirectToAction(nameof(Index));
            }

            // numero 2: Buscar el usuario en la base de datos y validar su existencia.
            var usuario = await _Context.Usuarios.FindAsync(model.PerfilID);
            if (usuario == null)
            {
                TempData["Error"] = "Usuario no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            // numero 3: Manejar la lógica de carga, validación y eliminación de la imagen.
            try
            {
                // numero 3.1: Validar tipo de archivo si se sube una imagen
                if (model.Imagen != null && model.Imagen.Length > 0)
                {
                    // Validar tamaño (máximo 2MB)
                    if (model.Imagen.Length > 2 * 1024 * 1024)
                    {
                        TempData["Error"] = "La imagen no debe superar los 2MB.";
                        return RedirectToAction(nameof(Index));
                    }

                    // Validar extensión de la imagen
                    var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(model.Imagen.FileName).ToLowerInvariant();

                    if (string.IsNullOrEmpty(extension) || !extensionesPermitidas.Contains(extension))
                    {
                        TempData["Error"] = "Solo se permiten imágenes JPG, PNG o GIF.";
                        return RedirectToAction(nameof(Index));
                    }
                    // Eliminar imagen anterior si existe
                    if (!string.IsNullOrEmpty(usuario.Imagen))
                    {
                        var rutaImagenAnterior = Path.Combine(_Environment.WebRootPath, usuario.Imagen.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                        if (System.IO.File.Exists(rutaImagenAnterior))
                        {
                            System.IO.File.Delete(rutaImagenAnterior);
                        }
                    }


                    // Crear directorio si no existe
                    var carpetaImagenes = Path.Combine(_Environment.WebRootPath,"img", "usuarios");
                    if (!Directory.Exists(carpetaImagenes))
                    {
                        Directory.CreateDirectory(carpetaImagenes);
                    }

                    // Generar nombre único y seguro
                    var nombreArchivo = $"{usuario.UsuarioId}_{Guid.NewGuid()}{extension}";
                    var rutaCompleta = Path.Combine(carpetaImagenes, nombreArchivo);

                    // Guardar imagen
                    using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                    {
                        await model.Imagen.CopyToAsync(stream);
                    }

                    // Guardar nueva ruta en la variable
                    usuario.Imagen = $"/img/usuarios/{nombreArchivo}"; ;
                }
                else if (!string.IsNullOrEmpty(usuario.Imagen) && model.Imagen == null)
                {
                    var rutaImagenActual = Path.Combine(_Environment.WebRootPath, usuario.Imagen.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (System.IO.File.Exists(rutaImagenActual))
                    {
                        System.IO.File.Delete(rutaImagenActual);
                    }
                    usuario.Imagen = null;
                }
                // numero 4: Guardar los cambios en la base de datos.
                await _Context.SaveChangesAsync();
                TempData["Success"] = "Imagen de perfil actualizada exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al actualizar la imagen de perfil.";
                
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
