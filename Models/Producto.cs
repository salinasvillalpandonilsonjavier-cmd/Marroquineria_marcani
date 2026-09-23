using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarroquineriaMarcani.Models
{
    [Table("productos")]
    public class Producto
    {
        [Key]
        [Column("id_producto")]
        public int IdProducto { get; set; }

        [Required]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("costo_fabricacion")]
        public decimal CostoFabricacion { get; set; }

        [Column("precio_venta")]
        public decimal PrecioVenta { get; set; }

        [Column("stock_disponible")]
        public int StockDisponible { get; set; }
    }
}