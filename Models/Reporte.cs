using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarroquineriaMarcani.Models
{
    [Table("Reportes")]
    public class Reporte
    {
        [Key]
        [Column("id_reporte")]
        public int IdReporte { get; set; }

        [Column("titulo")]
        public string Titulo { get; set; } = string.Empty;

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("fecha_envio")]
        public DateTime FechaEnvio { get; set; } = DateTime.Now;

        [Column("archivo_ruta")]
        public string? ArchivoRuta { get; set; }

        [Column("estado")]
        public string Estado { get; set; } = "Enviado";

        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }
    }
}