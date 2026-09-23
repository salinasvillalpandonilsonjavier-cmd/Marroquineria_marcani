using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarroquineriaMarcani.Data;
using MarroquineriaMarcani.Models;

namespace MarroquineriaMarcani.Controllers
{
    [Authorize]
    public class FlujoCajaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FlujoCajaController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var movimientos = await _context.FlujosCaja
                .OrderByDescending(f => f.Fecha)
                .ToListAsync();

            // Totales para las tarjetas superiores (Soporta minúsculas/mayúsculas)
            decimal totalIngresos = movimientos
                .Where(m => string.Equals(m.TipoMovimiento, "Ingreso", StringComparison.OrdinalIgnoreCase))
                .Sum(m => m.Monto);

            decimal totalEgresos = movimientos
                .Where(m => string.Equals(m.TipoMovimiento, "Egreso", StringComparison.OrdinalIgnoreCase))
                .Sum(m => m.Monto);

            ViewBag.TotalIngresos = totalIngresos;
            ViewBag.TotalEgresos = totalEgresos;
            ViewBag.SaldoTotal = totalIngresos - totalEgresos;

            return View(movimientos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(FlujoCaja flujo)
        {
            if (ModelState.IsValid)
            {
                flujo.Fecha = DateTime.Now;
                _context.FlujosCaja.Add(flujo);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var movimiento = await _context.FlujosCaja.FindAsync(id);
            if (movimiento != null)
            {
                _context.FlujosCaja.Remove(movimiento);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}