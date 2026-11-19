using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleInventario.Web.Models;

[Table("Productos")]
public class Productos
{
    [Key] public int ProductosId { get; set; }
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(300, ErrorMessage = "El nombre admite máximo 300 caracteres.")]
    [Display(Name = "Producto o servicio")]
    public string Nombre { get; set; } = string.Empty;
    [Required, Display(Name = "Unidad de medida")]
    public int FkUnidadesId { get; set; }
    [Required, Range(typeof(decimal), "0", "9999999")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Precio { get; set; }


    [Required]
    [Range(typeof(decimal), "0", "9999999999999999")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Stock mínimo")]
    public decimal StockMinimo { get; set; }

    [Required]
    [Range(typeof(decimal), "0", "9999999999999999")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Precio de compra")]
    public decimal PrecioCompra { get; set; }

    [Required, Range(typeof(decimal), "0", "9999999")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Existencia { get; set; }
    [ForeignKey(nameof(FkUnidadesId))] public Unidades? Unidades { get; set; }
    public ICollection<Entradas> Entradas { get; set; } = new List<Entradas>();
    public ICollection<Salidas> Salidas { get; set; } = new List<Salidas>();
}
