using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarroquineriaMarcani.Models
{
    [Table("LibroDiario")]
    public class LibroDiario
    {
        [Key]
        [Column("id_asiento")]
        public int id_asiento { get; set; }

        [Column("fecha")]
        public DateTime fecha { get; set; }

        [Column("concepto")]
        public string concepto { get; set; } = string.Empty;

        [Column("cuenta_contable")]
        public string cuenta_contable { get; set; } = string.Empty;

        [Column("debe")]
        public decimal debe { get; set; }

        [Column("haber")]
        public decimal haber { get; set; }

        [Column("usuario_id")]
        public int usuario_id { get; set; }
    }
} 