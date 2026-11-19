using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleInventario.Web.Data;
using SimpleInventario.Web.ViewModels;
using SimpleInventario.Web.Services;

namespace SimpleInventario.Web.Controllers;

public class HomeController(ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var model = new DashboardViewModel
        {
            TotalProductos = await db.Productos.CountAsync(ct),
            TotalUnidades = await db.Unidades.CountAsync(ct),
            TotalExistencias = await db.Productos.SumAsync(x => (decimal?)x.Existencia, ct) ?? 0,
            ValorInventario = await db.Productos.SumAsync(x => (decimal?)(x.Existencia * x.Precio), ct) ?? 0,
            TotalEntradas = await db.Entradas.CountAsync(ct),
            TotalSalidas = await db.Salidas.CountAsync(ct),
            TotalProductosSinExistencia = await db.Productos.CountAsync(p => p.Existencia <= 0, ct),
            TotalProductosPorResurtir = await db.Productos.CountAsync(p => p.StockMinimo > 0 &&p.Existencia <= p.StockMinimo,ct)
        };
        return View(model);
    }
    public IActionResult Error() => View();
}
