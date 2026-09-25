using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarroquineriaMarcani.Models
{
    [Table("ProductoInsumo")]
    public class ProductoInsumo
    {
        [Key]
        public int Id { get; set; }

        [Column("ProductoId")]
        public int ProductoId { get; set; }

        [Column("InsumoId")]
        public int InsumoId { get; set; }

        [Column("CantidadRequerida")]
        public decimal CantidadRequerida { get; set; }

        [ForeignKey("ProductoId")]
        public virtual Producto? Producto { get; set; }

        [ForeignKey("InsumoId")]
        public virtual Insumo? Insumo { get; set; }
    }
}