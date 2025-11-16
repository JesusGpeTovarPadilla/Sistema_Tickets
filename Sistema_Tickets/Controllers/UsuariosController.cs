using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Sistema_Tickets.Data;
using Sistema_Tickets.Models;
using Sistema_Tickets.ViewModels.Usuarios;

namespace Sistema_Tickets.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {

        //constructor
        private readonly TicketSystemDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public UsuariosController(TicketSystemDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        /*Metodos principales del controlador*/

        /*Metodos GET*/
        public async Task<IActionResult> Index()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Departamento)
                .ToListAsync();
            return View(usuarios);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["RolId"] = new SelectList(await _context.Roles.ToListAsync(), "RolId", "Nombre");
            ViewData["DepartamentoId"] = new SelectList(await _context.Departamentos.ToListAsync(), "DepartamentoId", "Nombre");
            return View();
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            var viewModel = new UsuarioEditViewModel
            {
                UsuarioId = usuario.UsuarioId,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                RolId = usuario.RolId,
                DepartamentoId = usuario.DepartamentoId,
                Estado = usuario.Estado,
                Esbloqueado = usuario.Esbloqueado,
                ImagenActual = usuario.Imagen
            };

            await CargarDropdowns(viewModel.RolId, viewModel.DepartamentoId);
            return View(viewModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Departamento)
                .FirstOrDefaultAsync(u => u.UsuarioId == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        /*Metodos POST*/

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioCreateViewmodel viewModel)
        {
            // Numero 1: se valida si el correo es unico correo único
            if (await _context.Usuarios.AnyAsync(u => u.Correo == viewModel.Correo))
            {
                ModelState.AddModelError("Correo", "Este correo ya está registrado");
            }

            //numero 2: se valida la imagen si se proporcionó
            if (viewModel.ImagenArchivo != null)
            {
                var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(viewModel.ImagenArchivo.FileName).ToLowerInvariant();

                if (!extensionesPermitidas.Contains(extension))
                {
                    ModelState.AddModelError("ImagenArchivo", "Solo se permiten imágenes (jpg, jpeg, png, gif)");
                }
                else if (viewModel.ImagenArchivo.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("ImagenArchivo", "La imagen no debe superar los 2MB");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ No se proporcionó ninguna imagen");
            }

            //numero 3: Si todo es válido, se procede a crear el usuario
            if (ModelState.IsValid)
            {
                //numero 3.1:  Mapear ViewModel a Entidad
                var usuario = new Usuario
                {
                    Nombre = viewModel.Nombre,
                    Correo = viewModel.Correo,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(viewModel.Password),
                    RolId = viewModel.RolId,
                    DepartamentoId = viewModel.DepartamentoId,
                    Estado = viewModel.Estado,
                    FechaAlta = DateTime.Now,
                    Esbloqueado = false,
                    UltimaSeccion = null
                };

                //Numero 3.2: Guardar la imagen en su ruta
                if (viewModel.ImagenArchivo != null && viewModel.ImagenArchivo.Length > 0)
                {
                    usuario.Imagen = await GuardarImagen(viewModel.ImagenArchivo);
                }

                //numero 3.3: Guardar en la base de datos
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                //numero 3.4: Redirigir con mensaje de éxito
                TempData["Success"] = "Usuario creado correctamente";
                return RedirectToAction(nameof(Index));
            }
            //numero 4: Si hay errores, recargar dropdowns y retornar la vista con errores
            await CargarDropdowns(viewModel.RolId, viewModel.DepartamentoId);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UsuarioEditViewModel viewModel)
        {
            System.Diagnostics.Debug.WriteLine($"🔍 Iniciando edición de usuario ID: {viewModel.UsuarioId}");
            // Validar correo único (excepto el mismo usuario)
            if (await _context.Usuarios.AnyAsync(u => u.Correo == viewModel.Correo && u.UsuarioId != viewModel.UsuarioId))
            {
                ModelState.AddModelError("Correo", "Este correo ya está registrado");
            }
            // Validar contraseña SOLO si se proporcionó
            if (!string.IsNullOrWhiteSpace(viewModel.Password))
            {
                System.Diagnostics.Debug.WriteLine($"Se proporcionó nueva contraseña");

                if (viewModel.Password.Length < 8)
                {
                    ModelState.AddModelError("Password", "La contraseña debe tener al menos 8 caracteres");
                }

                if (viewModel.Password != viewModel.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Las contraseñas no coinciden");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"No se proporcionó contraseña, se mantendrá la actual");
                // Remover errores de validación de contraseña si no se proporcionó
                ModelState.Remove("Password");
                ModelState.Remove("ConfirmPassword");
            }
            // Validar imagen si se proporcionó
            if (viewModel.ImagenArchivo != null)
            {
                System.Diagnostics.Debug.WriteLine($"📸 Se detectó una imagen: {viewModel.ImagenArchivo.FileName}");

                var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(viewModel.ImagenArchivo.FileName).ToLowerInvariant();

                if (!extensionesPermitidas.Contains(extension))
                {
                    ModelState.AddModelError("ImagenArchivo", "Solo se permiten imágenes (jpg, jpeg, png, gif)");
                }
                else if (viewModel.ImagenArchivo.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("ImagenArchivo", "La imagen no debe superar los 2MB");
                }
            }
            // Si todo es válido, proceder a actualizar
            if (ModelState.IsValid)
            {
                try
                {
                    var usuario = await _context.Usuarios.FindAsync(viewModel.UsuarioId);
                    if (usuario == null)
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ ERROR: Usuario con ID {viewModel.UsuarioId} no encontrado.");
                        return NotFound();
                    }
                    // Actualizar campos
                    usuario.Nombre = viewModel.Nombre;
                    usuario.Correo = viewModel.Correo;
                    usuario.RolId = viewModel.RolId;
                    usuario.DepartamentoId = viewModel.DepartamentoId;
                    usuario.Estado = viewModel.Estado;
                    usuario.Esbloqueado = viewModel.Esbloqueado;
                    // Actualizar contraseña si se proporcionó
                    if (!string.IsNullOrWhiteSpace(viewModel.Password))
                    {
                        usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(viewModel.Password);
                        System.Diagnostics.Debug.WriteLine($"🔒 Contraseña actualizada para el usuario ID: {viewModel.UsuarioId}");
                    }
                    // Manejar imagen
                    if (viewModel.EliminarImagen && !string.IsNullOrEmpty(usuario.Imagen))
                    {
                        var rutaImagenActual = Path.Combine(_environment.WebRootPath, usuario.Imagen.TrimStart('/'));
                        if (System.IO.File.Exists(rutaImagenActual))
                        {
                            System.IO.File.Delete(rutaImagenActual);
                            System.Diagnostics.Debug.WriteLine($"🗑️ Imagen actual eliminada: {rutaImagenActual}");
                        }
                        usuario.Imagen = null;
                    }
                    if (viewModel.ImagenArchivo != null && viewModel.ImagenArchivo.Length > 0)
                    {
                        usuario.Imagen = await GuardarImagen(viewModel.ImagenArchivo);
                    }
                    // Guardar cambios
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                    // Redirigir con mensaje de éxito
                    TempData["Success"] = "Usuario actualizado correctamente";
                    return RedirectToAction(nameof(Index));

                }
                catch
                {
                    System.Diagnostics.Debug.WriteLine($"❌ ERROR: Usuario con ID {viewModel.UsuarioId} no encontrado.");
                    return NotFound();
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"❌ ModelState inválido");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    System.Diagnostics.Debug.WriteLine($"   - {error.ErrorMessage}");
                }
            }

            // Recargar dropdowns y retornar vista con errores si los hay
            await CargarDropdowns(viewModel.RolId, viewModel.DepartamentoId);
            return View(viewModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // numero 1: Buscar el usuario y validar existencia y estado
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            if (!usuario.Estado)
            {
                TempData["Info"] = "El usuario ya estaba desactivado.";
                return RedirectToAction(nameof(Index));
            }
            // numero 2: actualizar el estado a inactivo
            usuario.Estado = false;

            // numero 3: guardar cambios
            _context.Update(usuario);
            await _context.SaveChangesAsync();

            // numero 4: redirigir con mensaje de éxito
            TempData["Success"] = "Usuario desactivado correctamente";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activation(int id)
        {
            // numero 1: Buscar el usuario y validar existencia y estado
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            if (usuario.Estado)
            {
                TempData["Info"] = "El usuario ya estaba activo.";
                return RedirectToAction(nameof(Index));
            }
            // numero 2: actualizar el estado a activo
            usuario.Estado = true;
            // numero 3: guardar cambios
            _context.Update(usuario);
            await _context.SaveChangesAsync();
            // numero 4: redirigir con mensaje de éxito
            TempData["Success"] = "Usuario activado correctamente";
            return RedirectToAction(nameof(Index));
        }

        /*Metodos auxiliares para los metodos principales*/

        // Metodo auxiliar para Guardar la imagen del usuario
        private async Task<string> GuardarImagen(IFormFile archivo)
        {
            try
            {
                // Log para debug
                System.Diagnostics.Debug.WriteLine($"📁 Intentando guardar imagen: {archivo.FileName}");
                System.Diagnostics.Debug.WriteLine($"📁 Tamaño del archivo: {archivo.Length} bytes");

                var uploadsFolder = Path.Combine(_environment.WebRootPath, "img", "usuarios");
                System.Diagnostics.Debug.WriteLine($"📁 WebRootPath: {uploadsFolder}");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
                var nombreUnico = $"{Guid.NewGuid()}{extension}";
                var rutaArchivo = Path.Combine(uploadsFolder, nombreUnico);

                using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                {
                    await archivo.CopyToAsync(stream);
                }

                System.Diagnostics.Debug.WriteLine($"📁 Imagen guardada en: {rutaArchivo}");

                var rutaRelativa = $"/img/usuarios/{nombreUnico}";
                System.Diagnostics.Debug.WriteLine($"📁 Ruta relativa: {rutaRelativa}");

                return rutaRelativa;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ERROR al guardar imagen: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"❌ StackTrace: {ex.StackTrace}");
                throw new Exception($"Error al guardar la imagen: {ex.Message}");
            }
        }
        // Metodo auxiliar para cargar los dropdowns
        private async Task CargarDropdowns(int? rolId = null, int? departamentoId = null)
        {
            var roles = await _context.Roles
                .OrderBy(r => r.Nombre)
                .ToListAsync();

            var departamentos = await _context.Departamentos
                .OrderBy(d => d.Nombre)
                .ToListAsync();

            ViewData["RolId"] = new SelectList(roles, "RolId", "Nombre", rolId);
            ViewData["DepartamentoId"] = new SelectList(departamentos, "DepartamentoId", "Nombre", departamentoId);
        }




    }
}
