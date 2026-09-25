using Microsoft.EntityFrameworkCore;
using MarroquineriaMarcani.Models;

namespace MarroquineriaMarcani.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Insumo> Insumos { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetalleVentas { get; set; }
        public DbSet<FlujoCaja> FlujoCaja { get; set; }
public DbSet<FlujoCaja> FlujoCajas => FlujoCaja; // Alias de compatibilidad
        public DbSet<ProductoInsumo> ProductoInsumo { get; set; }
public DbSet<LibroDiario> LibroDiario { get; set; }
public DbSet<Reporte> Reportes { get; set; }
    }
}