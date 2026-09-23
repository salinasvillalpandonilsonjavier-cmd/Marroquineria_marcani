using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarroquineriaMarcani.Controllers
{
    [Authorize]
    public class VentasController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}