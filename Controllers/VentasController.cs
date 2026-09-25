using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarroquineriaMarcani.Data;
using MarroquineriaMarcani.Models;
using ClosedXML.Excel;
using System.IO;

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

            // 1. Validar disponibilidad de stock del producto
            if (producto == null || producto.StockDisponible < cantidad)
            {
                TempData["Error"] = "Operación cancelada: Stock insuficiente de producto.";
                return RedirectToAction(nameof(Index));
            }

            // 2. Validar disponibilidad de INSUMOS requeridos (BOM - Receta)
            var recetaInsumos = await _context.ProductoInsumo
                .Include(pi => pi.Insumo)
                .Where(pi => pi.ProductoId == idProducto)
                .ToListAsync();

            foreach (var item in recetaInsumos)
            {
                decimal insumoRequeridoTotal = item.CantidadRequerida * cantidad;
                if (item.Insumo.StockActual < insumoRequeridoTotal)
                {
                    TempData["Error"] = $"Operación cancelada: Stock insuficiente del insumo '{item.Insumo.Nombre}'. Requerido: {insumoRequeridoTotal} {item.Insumo.UnidadMedida}, Disponible: {item.Insumo.StockActual}";
                    return RedirectToAction(nameof(Index));
                }
            }

            // Obtener usuario activo/registrado
            var usuario = await _context.Usuarios.FirstOrDefaultAsync();
            if (usuario == null)
            {
                TempData["Error"] = "Error: No existe ningún usuario registrado en la base de datos.";
                return RedirectToAction(nameof(Index));
            }

            // 3. Cálculos de Venta
            decimal totalVenta = producto.PrecioVenta * cantidad;
            decimal costoTotal = producto.CostoFabricacion * cantidad;
            decimal utilidadNeta = totalVenta - costoTotal;

            // 4. Descontar Stock del Producto y de los Insumos
            producto.StockDisponible -= cantidad;

            foreach (var item in recetaInsumos)
            {
                item.Insumo.StockActual -= (item.CantidadRequerida * cantidad);
            }

            // 5. Crear Venta
            var venta = new Venta
            {
                FechaVenta = DateTime.Now,
                Total = totalVenta,
                UtilidadNeta = utilidadNeta,
                IdUsuario = usuario.IdUsuario
            };
            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();

            // 6. Registrar en Flujo de Caja (Corregido)
            var flujo = new FlujoCaja
            {
                Fecha = DateTime.Now,
                Tipo = "Ingreso",
                Monto = totalVenta,
                Descripcion = $"Venta #{venta.IdVenta} - {cantidad}x {producto.Nombre}"
            };
            _context.FlujoCaja.Add(flujo);

            // 7. Registrar en Libro Diario
            var asientoDiario = new LibroDiario
            {
                fecha = DateTime.Now,
                concepto = $"Venta realizada: {cantidad}x {producto.Nombre}",
                cuenta_contable = "Ventas",
                debe = totalVenta,
                haber = 0.00m,
                usuario_id = usuario.IdUsuario
            };
            _context.LibroDiario.Add(asientoDiario);

            await _context.SaveChangesAsync();

            TempData["Exito"] = $"Venta e insumos registrados correctamente. Utilidad neta: {utilidadNeta:0.00} BOB";
            return RedirectToAction(nameof(Index));
        }

        // --- MÓDULO EXPORTACIÓN PARA EL CONTADOR / ADMIN ---
        [HttpGet]
        [Authorize(Roles = "Contador,Admin")]
        public async Task<IActionResult> ExportarVentasHoy()
        {
            var hoy = DateTime.Today;
            var ventasHoy = await _context.Ventas
                .Where(v => v.FechaVenta.Date == hoy)
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Ventas del Día");

                // Encabezados
                worksheet.Cell(1, 1).Value = "ID Venta";
                worksheet.Cell(1, 2).Value = "Fecha y Hora";
                worksheet.Cell(1, 3).Value = "Total (BOB)";
                worksheet.Cell(1, 4).Value = "Utilidad Neta (BOB)";

                // Formato de Encabezado
                var headerRange = worksheet.Range("A1:D1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#1F4E78");
                headerRange.Style.Font.FontColor = XLColor.White;

                // Filas
                int row = 2;
                foreach (var v in ventasHoy)
                {
                    worksheet.Cell(row, 1).Value = v.IdVenta;
                    worksheet.Cell(row, 2).Value = v.FechaVenta.ToString("dd/MM/yyyy HH:mm");
                    worksheet.Cell(row, 3).Value = v.Total;
                    worksheet.Cell(row, 4).Value = v.UtilidadNeta;
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Ventas_{hoy:yyyyMMdd}.xlsx");
                }
            }
        }
    }
}