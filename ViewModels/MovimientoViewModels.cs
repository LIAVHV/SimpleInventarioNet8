using SimpleInventario.Web.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleInventario.Web.ViewModels;


public class EntradaViewModel
{
    [Required(ErrorMessage = "Selecciona un producto.")]
    [Range(1, int.MaxValue)]
    [Display(Name = "Producto")]
    public int ProductoId { get; set; }

    [Required(ErrorMessage = "Selecciona un proveedor.")]
    [Range(1, int.MaxValue)]
    [Display(Name = "Proveedor")]
    public int ProveedorId { get; set; }

    [Required]
    [Range(typeof(decimal), "0.01", "9999999999999999")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Precio de compra")]
    public decimal Precio { get; set; }

    [Required]
    [Range(typeof(decimal), "0.01", "9999999999999999")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Precio de venta")]
    public decimal PrecioVenta { get; set; }

    [Required]
    [Range(typeof(decimal), "0.01", "9999999999999999")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Cantidad")]
    public decimal Cantidad { get; set; }
}

public class SalidaViewModel
{
    [Required(ErrorMessage = "Selecciona un producto.")]
    [Range(1, int.MaxValue)]
    [Display(Name = "Producto")]
    public int ProductoId { get; set; }

    [Required]
    [Range(typeof(decimal), "0.01", "9999999999999999")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Precio de venta")]
    public decimal PrecioVenta { get; set; }

    [Required]
    [Range(typeof(decimal), "0.01", "9999999999999999")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Cantidad a retirar")]
    public decimal Cantidad { get; set; }
}


public class KardexItemViewModel
{
    public int Folio { get; set; }
    public string Nombre { get; set; } = "";
    public string Unidad { get; set; } = "";
    public decimal Precio { get; set; }
    public decimal Cantidad { get; set; }
    public decimal TotalImporte { get; set; }
    public string Tipo { get; set; } = "";
    public DateTime Fecha { get; set; }
}

public class DashboardViewModel
{
    public int TotalProductos { get; set; }
    public int TotalUnidades { get; set; }
    public decimal TotalExistencias { get; set; }
    public decimal ValorInventario { get; set; }
    public int TotalEntradas { get; set; }
    public int TotalSalidas { get; set; }
    public int TotalProductosSinExistencia { get; set; }
    public int TotalProductosPorResurtir { get; set; }

}


public class DashboardProductoSinExistenciaViewModel
{
    public int ProductoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Unidad { get; set; } = string.Empty;
    public decimal Precio { get; set; }
}
