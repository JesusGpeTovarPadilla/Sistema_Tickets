using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Sistema_Tickets.Models;
using System.Security.Claims;
namespace Sistema_Tickets.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Login", "Auth");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult RedirectToHome()
        {
            
            
            var rolUsuario = User.FindFirst(ClaimTypes.Role)?.Value;

            return rolUsuario switch
            {
                "Administrador" => RedirectToAction("Index", "Admin"),
                "Gerente" => RedirectToAction("Index", "Gerente"),
                "Técnico" => RedirectToAction("Index", "Tecnico"),
                "Usuario" => RedirectToAction("Index", "Usuario"),
                _ => RedirectToAction("Dashboard", "Home")
            };
        }
    }
}
