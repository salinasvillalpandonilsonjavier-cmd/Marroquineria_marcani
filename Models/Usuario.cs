using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarroquineriaMarcani.Models
{
    [Table("usuarios")]
    public class Usuario
    {
        [Key]
        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Required]
        [Column("nombre_completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required]
        [Column("correo")]
        public string Correo { get; set; } = string.Empty;

        [Required]
        [Column("contrasena_hash")]
        public string ContrasenaHash { get; set; } = string.Empty;

        [Column("id_rol")]
        public int IdRol { get; set; }

        [ForeignKey("IdRol")]
        public Rol? Rol { get; set; }
    }
}