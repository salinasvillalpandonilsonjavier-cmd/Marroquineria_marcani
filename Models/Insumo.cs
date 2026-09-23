using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarroquineriaMarcani.Models
{
    [Table("insumos")]
    public class Insumo
    {
        [Key]
        [Column("id_insumo")]
        public int IdInsumo { get; set; }

        [Required]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("unidad_medida")]
        public string? UnidadMedida { get; set; }

        [Column("stock_actual")]
        public int StockActual { get; set; }

        [Column("stock_minimo")]
        public int StockMinimo { get; set; }

        [Column("costo_unitario")]
        public decimal CostoUnitario { get; set; }
    }
}