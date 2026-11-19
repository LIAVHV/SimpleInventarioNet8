
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleInventario.Web.Models;

[Table("Proveedores")]
public class Proveedores
{
    [Key]
    public int ProveedoresId { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150)]
    [Display(Name = "Nombre del proveedor")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [StringLength(30)]
    [Display(Name = "Teléfono de contacto")]
    public string Telefono { get; set; } = string.Empty;

    public ICollection<Entradas> Entradas { get; set; }
        = new List<Entradas>();
}

