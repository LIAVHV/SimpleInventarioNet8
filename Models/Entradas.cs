using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleInventario.Web.Models;

[Table("Entradas")]
public class Entradas
{
    [Key] public int EntradasId { get; set; }
    [Required]
    public int FKProveedorId { get; set; }
    [Required] public int FKProductosId { get; set; }
    [Required, Range(typeof(decimal), "0.01", "9999999999999999")]
    [Column(TypeName = "decimal(18,2)")] public decimal Precio { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Precio de venta")]
    public decimal PrecioVenta { get; set; }
    [Required, Range(typeof(decimal), "0.01", "9999999999999999")]
    [Column(TypeName = "decimal(18,2)")] public decimal Cantidad { get; set; }
    [Required, Column(TypeName = "datetime")] public DateTime Fecha { get; set; }
    [Required, Column(TypeName = "decimal(18,2)")] public decimal TotalImporte { get; set; }
    [ForeignKey(nameof(FKProductosId))] public Productos? Productos { get; set; }
    [ForeignKey(nameof(FKProveedorId))]
    public Proveedores? Proveedores { get; set; }
}
