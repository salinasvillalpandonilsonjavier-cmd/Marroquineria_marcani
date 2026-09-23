using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarroquineriaMarcani.Data;
using MarroquineriaMarcani.Models;

namespace MarroquineriaMarcani.Controllers
{
    public class InsumosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InsumosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Insumos
        public async Task<IActionResult> Index()
        {
            var insumos = await _context.Insumos.ToListAsync();
            return View(insumos);
        }

        // POST: Insumos/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Insumo insumo)
        {
            if (ModelState.IsValid)
            {
                _context.Insumos.Add(insumo);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Insumo registrado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            
            var listaInsumos = await _context.Insumos.ToListAsync();
            return View("Index", listaInsumos);
        }
    }
}