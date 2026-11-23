using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleInventario.Web.Models;

[Table("Unidades")]
public class Unidades
{
    [Key] 
    public int UnidadesId { get; set; }
    
    [Required(ErrorMessage = "La unidad es obligatoria.")]
    [StringLength(50, ErrorMessage = "La unidad admite máximo 50 caracteres.")]
    [Display(Name = "Unidad de medida")]
    public string Unidad { get; set; } = string.Empty;
    public ICollection<Productos> Productos { get; set; } = new List<Productos>();
}
