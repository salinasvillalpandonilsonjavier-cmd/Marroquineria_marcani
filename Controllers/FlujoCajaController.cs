using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarroquineriaMarcani.Data;
using MarroquineriaMarcani.Models;
using ClosedXML.Excel;

namespace MarroquineriaMarcani.Controllers
{
    [Authorize(Roles = "Contador,Admin,Administrador")]
    public class FlujoCajaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FlujoCajaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // VISTA PRINCIPAL DEL FLUJO DE CAJA
        public async Task<IActionResult> Index()
        {
            var movimientos = await _context.FlujoCaja
                .OrderByDescending(f => f.Fecha)
                .ToListAsync();

            ViewBag.TotalIngresos = movimientos.Where(m => m.Tipo == "Ingreso").Sum(m => m.Monto);
            ViewBag.TotalEgresos = movimientos.Where(m => m.Tipo == "Egreso").Sum(m => m.Monto);
            ViewBag.SaldoDisponible = (decimal)ViewBag.TotalIngresos - (decimal)ViewBag.TotalEgresos;

            return View(movimientos);
        }

        // ACCIÓN PARA DESCARGAR EL REPORTE EN EXCEL (.XLSX)
        [HttpGet]
        public async Task<IActionResult> ExportarExcel()
        {
            var movimientos = await _context.FlujoCaja
                .OrderByDescending(f => f.Fecha)
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Flujo de Caja");

                // Encabezados
                worksheet.Cell(1, 1).Value = "Fecha";
                worksheet.Cell(1, 2).Value = "Tipo";
                worksheet.Cell(1, 3).Value = "Descripción";
                worksheet.Cell(1, 4).Value = "Monto (BOB)";

                // Estilo del Encabezado (Marrón Marcani)
                var headerRange = worksheet.Range("A1:D1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#4A2511");
                headerRange.Style.Font.FontColor = XLColor.White;

                int row = 2;
                foreach (var item in movimientos)
                {
                    worksheet.Cell(row, 1).Value = item.Fecha.ToString("dd/MM/yyyy HH:mm");
                    worksheet.Cell(row, 2).Value = item.Tipo;
                    worksheet.Cell(row, 3).Value = item.Descripcion;
                    worksheet.Cell(row, 4).Value = item.Monto;
                    worksheet.Cell(row, 4).Style.NumberFormat.Format = "#,##0.00";
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"Flujo_de_Caja_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                    );
                }
            }
        }
    }
}