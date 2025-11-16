using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_Tickets.Data;
using Sistema_Tickets.ViewModels.Departamentos;

namespace Sistema_Tickets.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class DepartamentosController : Controller
    {
        private readonly TicketSystemDbContext _Context;

        public DepartamentosController(TicketSystemDbContext context)
        {
            _Context = context;
        }
        public async Task<IActionResult> Index()
        {
            var Departamentos = await _Context.Departamentos.ToListAsync();
            return View(Departamentos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CrearDepartamentoViewModel model)
        {
            if(!ModelState.IsValid)
            {
                TempData["info"] = "Por favor corrija los errores en el formulario.";
                return RedirectToAction(nameof(Index));
            }
            if (await _Context.Departamentos.AnyAsync(d => d.Nombre == model.Nombre))
            {
               TempData["info"] = "Ya existe un departamento con este nombre.";
                return RedirectToAction(nameof(Index));
            }
            var departamento = new Models.Departamento
            {
                Nombre = model.Nombre,
                Descripcion = model.Descripcion,
                Email = model.Email,
                Estatus = model.Estatus
            };
            _Context.Departamentos.Add(departamento);
            await _Context.SaveChangesAsync();
            TempData["Success"] = "Departamento creado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditarDepartamentoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["info"] = "Por favor corrija los errores en el formulario.";
                return RedirectToAction(nameof(Index));
            }
            if (await _Context.Departamentos.AnyAsync(d => d.Nombre == model.Nombre && d.DepartamentoId != model.DepartamentoId))
            {
                TempData["info"] = "Ya existe un departamento con este nombre.";
                return RedirectToAction(nameof(Index));

            }
            var departamento = await _Context.Departamentos.FindAsync(model.DepartamentoId);
            if (departamento == null)
            {
                return NotFound();
            }
            departamento.Nombre = model.Nombre;
            departamento.Descripcion = model.Descripcion;
            departamento.Email = model.Email;
            departamento.Estatus = model.Estatus;
            await _Context.SaveChangesAsync();
            TempData["Success"] = "Departamento actualizado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Status(int id)
        {
            var departamento= await _Context.Departamentos.FindAsync(id);
            if (departamento == null)
            {
                return NotFound();
            }
            if (departamento.Estatus==true)
            {
                departamento.Estatus = false;
                await _Context.SaveChangesAsync();
                TempData["Success"] = "Departamento desactivado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                departamento.Estatus = true;
                await _Context.SaveChangesAsync();
                TempData["Success"] = "Departamento activado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
        }
          
          
    }
}
