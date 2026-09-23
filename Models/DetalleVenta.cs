using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarroquineriaMarcani.Models
{
    [Table("detalle_ventas")]
    public class DetalleVenta
    {
        [Key]
        [Column("id_detalle")]
        public int IdDetalle { get; set; }

        [Column("id_venta")]
        public int IdVenta { get; set; }

        [Column("id_producto")]
        public int IdProducto { get; set; }

        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Column("precio_unitario")]
        public decimal PrecioUnitario { get; set; }

        [Column("subtotal")]
        public decimal Subtotal { get; set; }
    }
}