using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarroquineriaMarcani.Models
{
    [Table("flujo_caja")]
    public class FlujoCaja
    {
        [Key]
        [Column("id_movimiento")]
        public int IdMovimiento { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required]
        [Column("tipo_movimiento")]
        public string TipoMovimiento { get; set; } = "Ingreso";

        [Column("monto")]
        public decimal Monto { get; set; }

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("id_venta")]
        public int? IdVenta { get; set; }
    }
}