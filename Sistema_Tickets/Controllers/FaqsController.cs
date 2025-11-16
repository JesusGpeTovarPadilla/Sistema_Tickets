using Microsoft.AspNetCore.Mvc;

namespace Sistema_Tickets.Controllers
{
    public class FaqsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
