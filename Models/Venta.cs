using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarroquineriaMarcani.Models
{
    [Table("ventas")]
    public class Venta
    {
        [Key]
        [Column("id_venta")]
        public int IdVenta { get; set; }

        [Column("fecha_venta")]
        public DateTime FechaVenta { get; set; } = DateTime.Now;

        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Column("total")]
        public decimal Total { get; set; }

        [Column("utilidad_neta")]
        public decimal UtilidadNeta { get; set; }
    }
}