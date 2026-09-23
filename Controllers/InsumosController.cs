using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarroquineriaMarcani.Data;
using MarroquineriaMarcani.Models;

namespace MarroquineriaMarcani.Controllers
{
    [Authorize]
    public class InsumosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InsumosController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var insumos = await _context.Insumos.ToListAsync();
            return View(insumos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Insumo insumo)
        {
            if (ModelState.IsValid)
            {
                _context.Insumos.Add(insumo);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var insumo = await _context.Insumos.FindAsync(id);
            if (insumo != null)
            {
                _context.Insumos.Remove(insumo);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}