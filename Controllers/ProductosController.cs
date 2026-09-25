using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarroquineriaMarcani.Data;
using MarroquineriaMarcani.Models;
using System.Globalization;

namespace MarroquineriaMarcani.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductosController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Insumos = await _context.Insumos.ToListAsync();
            var productos = await _context.Productos.ToListAsync();
            return View(productos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(string Nombre, string PrecioVenta, int StockAFabricar, List<InsumoDetalleInput> Insumos)
        {
            // Parsear PrecioVenta usando punto o coma de forma segura
            string precioVentaLimpio = PrecioVenta?.Replace(',', '.') ?? "0";
            if (!decimal.TryParse(precioVentaLimpio, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal precioVentaParsed))
            {
                ModelState.AddModelError("PrecioVenta", "El precio de venta no tiene un formato válido.");
            }

            if (string.IsNullOrWhiteSpace(Nombre))
            {
                ModelState.AddModelError("Nombre", "El nombre es obligatorio.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Insumos = await _context.Insumos.ToListAsync();
                var listaProductos = await _context.Productos.ToListAsync();
                return View("Index", listaProductos);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                decimal costoFabricacionTotalPorUnidad = 0;
                var listaInsumosProcesados = new List<(int InsumoId, decimal Cantidad)>();

                if (Insumos != null && Insumos.Any())
                {
                    foreach (var item in Insumos)
                    {
                        var insumo = await _context.Insumos.FindAsync(item.InsumoId);
                        if (insumo == null) continue;

                        // Parsear CantidadRequerida de forma segura
                        string cantidadLimpia = item.CantidadRequeridaStr?.Replace(',', '.') ?? "0";
                        decimal.TryParse(cantidadLimpia, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal cantidadParsed);

                        decimal insumoTotalRequerido = cantidadParsed * StockAFabricar;

                        if (insumo.StockActual < insumoTotalRequerido)
                        {
                            ModelState.AddModelError(string.Empty, $"Stock insuficiente de '{insumo.Nombre}'. Requerido: {insumoTotalRequerido}, Disponible: {insumo.StockActual}");
                            ViewBag.Insumos = await _context.Insumos.ToListAsync();
                            var listaProds = await _context.Productos.ToListAsync();
                            return View("Index", listaProds);
                        }

                        // Descontar del inventario de insumos
                        insumo.StockActual -= insumoTotalRequerido;

                        // Acumular costo de fabricación unitario
                        costoFabricacionTotalPorUnidad += (cantidadParsed * insumo.CostoUnitario);

                        listaInsumosProcesados.Add((item.InsumoId, cantidadParsed));
                    }
                }

                // Guardar Producto
                var nuevoProducto = new Producto
                {
                    Nombre = Nombre,
                    PrecioVenta = precioVentaParsed,
                    CostoFabricacion = costoFabricacionTotalPorUnidad,
                    StockDisponible = StockAFabricar
                };

                _context.Productos.Add(nuevoProducto);
                await _context.SaveChangesAsync();

                // Guardar las relaciones ProductoInsumo
                foreach (var item in listaInsumosProcesados)
                {
                    var relacion = new ProductoInsumo
                    {
                        ProductoId = nuevoProducto.IdProducto,
                        InsumoId = item.InsumoId,
                        CantidadRequerida = item.Cantidad
                    };
                    _context.ProductoInsumo.Add(relacion);
                }
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError(string.Empty, "Error al procesar la fabricación del producto.");
                ViewBag.Insumos = await _context.Insumos.ToListAsync();
                var productos = await _context.Productos.ToListAsync();
                return View("Index", productos);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                var relaciones = _context.ProductoInsumo.Where(pi => pi.ProductoId == id);
                _context.ProductoInsumo.RemoveRange(relaciones);

                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }

    public class InsumoDetalleInput
    {
        public int InsumoId { get; set; }
        public string CantidadRequeridaStr { get; set; } = string.Empty;
    }
}