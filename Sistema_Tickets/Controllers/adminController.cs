using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_Tickets.Data;
using Sistema_Tickets.Models;

namespace Sistema_Tickets.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class adminController : Controller
    {
        private readonly TicketSystemDbContext _Context;

        public adminController(TicketSystemDbContext context)
        {
            _Context = context;
        }

        public async Task<IActionResult> Index()
        {
            var cantidadUsuarios= await _Context.Usuarios.CountAsync();
            var cantidadRoles = await _Context.Roles.CountAsync();
            var cantidadTickets= await _Context.Tickets.CountAsync();
            var cantidadDepartamento= await _Context.Departamentos.CountAsync();

            ViewBag.Usuarios = cantidadUsuarios;
            ViewBag.Roles = cantidadRoles;
            ViewBag.Tickets = cantidadTickets;
            ViewBag.Departamentos= cantidadDepartamento;
            return View();
        } 
    }
}
