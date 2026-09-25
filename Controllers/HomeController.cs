using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarroquineriaMarcani.Data;

namespace MarroquineriaMarcani.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Suma total de unidades físicas en stock de todos los productos (0 si no hay stock)
            var stockTotalProductos = await _context.Productos.SumAsync(p => (int?)p.StockDisponible) ?? 0;
            ViewBag.StockTotalProductos = stockTotalProductos;

            // 2. Conteo de tipos de insumos registrados
            ViewBag.TotalInsumos = await _context.Insumos.CountAsync();

            // 3. Insumos críticos con bajo stock (StockActual <= StockMinimo)
            var insumosCriticos = await _context.Insumos
                .Where(i => i.StockActual <= i.StockMinimo)
                .ToListAsync();

            ViewBag.InsumosCriticos = insumosCriticos;
            ViewBag.TotalInsumosAlerta = insumosCriticos.Count;

            return View();
        }
    }
}