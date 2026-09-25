using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarroquineriaMarcani.Data;
using MarroquineriaMarcani.Models;

namespace MarroquineriaMarcani.Controllers
{
    [Authorize]
    public class ReportesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ReportesController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // VISTA ADMIN: Acepta tanto "Admin" como "Administrador"
        [Authorize(Roles = "Admin,Administrador")]
        public async Task<IActionResult> Recibidos()
        {
            var reportes = await _context.Reportes
                .Include(r => r.Usuario)
                .OrderByDescending(r => r.FechaEnvio)
                .ToListAsync();

            return View(reportes);
        }

        // DESCARGA DE ARCHIVO PARA EL ADMIN
        [HttpGet]
        [Authorize(Roles = "Admin,Administrador")]
        public IActionResult Descargar(int id)
        {
            var reporte = _context.Reportes.FirstOrDefault(r => r.IdReporte == id);
            if (reporte == null || string.IsNullOrEmpty(reporte.ArchivoRuta))
            {
                return NotFound();
            }

            string filePath = Path.Combine(_environment.WebRootPath, reporte.ArchivoRuta.TrimStart('/'));
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            string fileName = Path.GetFileName(filePath);
            return File(fileBytes, "application/octet-stream", fileName);
        }

        // VISTA CONTADOR: Enviar reporte
        [Authorize(Roles = "Contador")]
        public IActionResult Enviar()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Contador")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enviar(string titulo, string descripcion, IFormFile? archivo)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync();
            if (usuario == null) return RedirectToAction("Index", "Home");

            string? rutaRelativa = null;

            if (archivo != null && archivo.Length > 0)
            {
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "reportes");
                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + archivo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await archivo.CopyToAsync(fileStream);
                }

                rutaRelativa = "/uploads/reportes/" + uniqueFileName;
            }

            var reporte = new Reporte
            {
                Titulo = titulo,
                Descripcion = descripcion,
                FechaEnvio = DateTime.Now,
                ArchivoRuta = rutaRelativa,
                Estado = "Enviado",
                UsuarioId = usuario.IdUsuario
            };

            _context.Reportes.Add(reporte);
            await _context.SaveChangesAsync();

            TempData["Exito"] = "Reporte enviado al Administrador con éxito.";
            return RedirectToAction(nameof(Enviar));
        }
    }
}