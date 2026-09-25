using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarroquineriaMarcani.Models
{
    [Table("flujocajas")]
    public class FlujoCaja
    {
        [Key]
        [Column("id_flujo_caja")]
        public int IdFlujoCaja { get; set; }

        [Column("tipo")]
        public string Tipo { get; set; } = string.Empty;

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("monto")]
        public decimal Monto { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}