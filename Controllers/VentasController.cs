using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarroquineriaMarcani.Data;
using MarroquineriaMarcani.Models;

namespace MarroquineriaMarcani.Controllers
{
    [Authorize]
    public class VentasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VentasController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var ventas = await _context.Ventas.OrderByDescending(v => v.FechaVenta).ToListAsync();
            ViewBag.Productos = await _context.Productos.Where(p => p.StockDisponible > 0).ToListAsync();
            return View(ventas);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(int idProducto, int cantidad)
        {
            var producto = await _context.Productos.FindAsync(idProducto);

            // HU04: Validar disponibilidad de stock
            if (producto == null || producto.StockDisponible < cantidad)
            {
                TempData["Error"] = "Operación cancelada: Stock insuficiente de producto.";
                return RedirectToAction(nameof(Index));
            }

            // Obtener un usuario de la base de datos para la relación con Venta
            var usuario = await _context.Usuarios.FirstOrDefaultAsync();

            if (usuario == null)
            {
                TempData["Error"] = "Error: No existe ningún usuario registrado en la base de datos.";
                return RedirectToAction(nameof(Index));
            }

            // HU03: Cálculo de Total y Utilidad Neta
            decimal totalVenta = producto.PrecioVenta * cantidad;
            decimal costoTotal = producto.CostoFabricacion * cantidad;
            decimal utilidadNeta = totalVenta - costoTotal;

            // 1. Descontar stock del producto
            producto.StockDisponible -= cantidad;

            // 2. Crear registro de venta asignando el IdUsuario obligatorio
            var venta = new Venta
            {
                FechaVenta = DateTime.Now,
                Total = totalVenta,
                UtilidadNeta = utilidadNeta,
                IdUsuario = usuario.IdUsuario
            };
            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();

            // 3. Registrar en Flujo de Caja
            var flujo = new FlujoCaja
            {
                Fecha = DateTime.Now,
                TipoMovimiento = "Ingreso",
                Monto = totalVenta,
                Descripcion = $"Venta de {cantidad}x {producto.Nombre}",
                IdVenta = venta.IdVenta
            };
            _context.FlujosCaja.Add(flujo);

            await _context.SaveChangesAsync();

            TempData["Exito"] = $"Venta registrada correctamente. Utilidad neta generada: {utilidadNeta:0.00} BOB";
            return RedirectToAction(nameof(Index));
        }
    }
}