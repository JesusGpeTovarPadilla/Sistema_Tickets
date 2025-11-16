using Microsoft.AspNetCore.Mvc;
using Sistema_Tickets.Data;

namespace Sistema_Tickets.Controllers
{
    public class RolesController : Controller
    {
        private readonly TicketSystemDbContext _Context;

        public RolesController(TicketSystemDbContext context)
        {
            _Context = context;
        }
        public async Task<IActionResult> Index()
        {
            var roles = _Context.Roles.ToList();
            return View(roles);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Descripcion,Estatus")] Models.Role role)
        {
            if (ModelState.IsValid)
            {
                _Context.Add(role);
                await _Context.SaveChangesAsync();
                TempData["Success"] = "Rol creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            TempData["info"] = "No se puede crear el rol.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RolId,Nombre,Descripcion,Estatus")] Models.Role role)
        {
            if(ModelState.IsValid)
            {
                var existingRole = await _Context.Roles.FindAsync(id);
                if (existingRole == null)
                {
                    return NotFound();
                }
                existingRole.Nombre = role.Nombre;
                existingRole.Descripcion = role.Descripcion;
                existingRole.Estatus = role.Estatus;
                await _Context.SaveChangesAsync();
                TempData["Success"] = "Rol actualizado correctamente.";
                return RedirectToAction(nameof(Index));

            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _Context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            if (!role.Estatus)
            {
                TempData["Info"] = "El usuario ya estaba desactivado.";
                return RedirectToAction(nameof(Index));
            }
            role.Estatus = false;
            await _Context.SaveChangesAsync();
            TempData["Success"] = "Rol desactivado correctamente.";
            return RedirectToAction(nameof(Index));



        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id)
        {
            var role = await _Context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            if (role.Estatus)
            {
                TempData["Info"] = "El rol ya estaba activado.";
                return RedirectToAction(nameof(Index));
            }
            role.Estatus = true;
            await _Context.SaveChangesAsync();
            TempData["Success"] = "Rol activado correctamente.";
            return RedirectToAction(nameof(Index));
        }
}
}
