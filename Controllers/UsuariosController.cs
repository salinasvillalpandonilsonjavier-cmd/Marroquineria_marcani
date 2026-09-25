using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarroquineriaMarcani.Data;
using MarroquineriaMarcani.Models;

namespace MarroquineriaMarcani.Controllers
{
    [Authorize(Roles = "Admin,Administrador")]
    public class UsuariosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Vista Principal de Usuarios
        public async Task<IActionResult> Index()
        {
            var usuarios = await _context.Usuarios.ToListAsync();
            ViewBag.Roles = await _context.Roles.ToListAsync();
            return View(usuarios);
        }

        // Accion POST para Crear Usuario desde el Modal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Usuario creado exitosamente.";
            }
            else
            {
                TempData["Error"] = "Ocurrió un error al intentar crear el usuario. Verifique los datos.";
            }

            return RedirectToAction(nameof(Index));
        }

        // Accion POST para Eliminar Usuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                TempData["Error"] = "El usuario no existe.";
                return RedirectToAction(nameof(Index));
            }

            // Proteccion de Backend: Bloquea la eliminacion si intentan eliminar la cuenta activa
            if (usuario.Correo == User.Identity?.Name || usuario.NombreCompleto == User.Identity?.Name)
            {
                TempData["Error"] = "Acción denegada: No puedes eliminar tu propio usuario activo.";
                return RedirectToAction(nameof(Index));
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            TempData["Exito"] = "Usuario eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}