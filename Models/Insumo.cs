using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarroquineriaMarcani.Models
{
    [Table("insumos")]
    public class Insumo
    {
        [Key]
        [Column("Id_insumo")]
        public int IdInsumo { get; set; }

        [Required(ErrorMessage = "El nombre del insumo es obligatorio.")]
        [Column("Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("Stock_actual")]
        public decimal StockActual { get; set; }

        [Column("Stock_minimo")]
        public decimal StockMinimo { get; set; }

        [Column("Unidad_medida")]
        public string UnidadMedida { get; set; } = "Unidad";

        [Column("Costo_unitario")]
        public decimal CostoUnitario { get; set; }
    }
}