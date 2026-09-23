using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarroquineriaMarcani.Data;

namespace MarroquineriaMarcani.Controllers
{
    [Authorize] // <--- ESTO OBLIGA A IR AL LOGIN PRIMERO SI NO ESTÁS AUTENTICADO
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Métricas y alertas estáticas/básicas para Sprint 1
            ViewBag.SaldoCaja = 0.00m;
            ViewBag.TotalVentas = 0.00m;
            ViewBag.TotalProductos = 0;
            ViewBag.TotalInsumosAlerta = 0;

            var insumosCriticos = await _context.Insumos
                .Where(i => i.StockActual <= i.StockMinimo)
                .ToListAsync();

            ViewBag.InsumosCriticos = insumosCriticos;

            return View();
        }
    }
}