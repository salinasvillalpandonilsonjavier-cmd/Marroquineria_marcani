using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarroquineriaMarcani.Data;
using MarroquineriaMarcani.Models;

namespace MarroquineriaMarcani.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Carga la lista de productos
        public async Task<IActionResult> Index()
        {
            var productos = await _context.Productos.ToListAsync();
            return View(productos);
        }

        // Recibe los datos del modal y guarda el producto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Producto producto)
        {
            if (ModelState.IsValid)
            {
                _context.Productos.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var productos = await _context.Productos.ToListAsync();
            return View("Index", productos);
        }

        // Elimina el producto por ID
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}