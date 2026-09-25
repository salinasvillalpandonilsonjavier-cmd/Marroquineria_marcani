using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarroquineriaMarcani.Data;
using MarroquineriaMarcani.Models;
using System.Globalization;

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
        public async Task<IActionResult> Crear(Insumo insumo, string CostoUnitarioStr, string StockActualStr, string StockMinimoStr)
        {
            try
            {
                var cultura = CultureInfo.InvariantCulture;

                if (!string.IsNullOrEmpty(CostoUnitarioStr))
                    insumo.CostoUnitario = decimal.Parse(CostoUnitarioStr.Replace(',', '.'), cultura);

                if (!string.IsNullOrEmpty(StockActualStr))
                    insumo.StockActual = decimal.Parse(StockActualStr.Replace(',', '.'), cultura);

                if (!string.IsNullOrEmpty(StockMinimoStr))
                    insumo.StockMinimo = decimal.Parse(StockMinimoStr.Replace(',', '.'), cultura);

                _context.Insumos.Add(insumo);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Insumo registrado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al guardar el insumo: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Insumos/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, string Nombre, string UnidadMedida, string CostoUnitarioStr, string StockActualStr, string StockMinimoStr)
        {
            var insumo = await _context.Insumos.FindAsync(id);
            if (insumo == null)
            {
                return NotFound();
            }

            try
            {
                var cultura = CultureInfo.InvariantCulture;

                insumo.Nombre = Nombre;
                insumo.UnidadMedida = UnidadMedida;

                if (!string.IsNullOrEmpty(CostoUnitarioStr))
                    insumo.CostoUnitario = decimal.Parse(CostoUnitarioStr.Replace(',', '.'), cultura);

                if (!string.IsNullOrEmpty(StockActualStr))
                    insumo.StockActual = decimal.Parse(StockActualStr.Replace(',', '.'), cultura);

                if (!string.IsNullOrEmpty(StockMinimoStr))
                    insumo.StockMinimo = decimal.Parse(StockMinimoStr.Replace(',', '.'), cultura);

                _context.Insumos.Update(insumo);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Insumo actualizado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al actualizar el insumo: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Insumos/Eliminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var insumo = await _context.Insumos.FindAsync(id);
            if (insumo != null)
            {
                try
                {
                    _context.Insumos.Remove(insumo);
                    await _context.SaveChangesAsync();
                    TempData["Exito"] = "Insumo eliminado correctamente.";
                }
                catch (Exception)
                {
                    TempData["Error"] = "No se puede eliminar este insumo porque está asociado a la receta de un producto existente.";
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}