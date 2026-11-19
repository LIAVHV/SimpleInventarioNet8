using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SimpleInventario.Web.Data;
using SimpleInventario.Web.Services;
using SimpleInventario.Web.Models;

namespace SimpleInventario.Web.Controllers;

public class ReportesController(ApplicationDbContext db, IReportesService reportes) : Controller
{
    public async Task<IActionResult> Existencias(string? q, int? pageIndex, int? itemsPorPagina, CancellationToken ct)
    {
        var query = db.Productos.AsNoTracking().Include(x => x.Unidades).Where(x => x.Existencia > 0).AsQueryable();
        if (!string.IsNullOrWhiteSpace(q)) query = query.Where(x => x.Nombre.Contains(q.Trim()));
        ViewBag.Query = q;
        var resultado = await Paginar<Productos>.PaginarEntidad(query.OrderBy(x => x.Nombre), pageIndex, itemsPorPagina, ct);
        return View(resultado);
    }
    public async Task<IActionResult> Kardex(int? productoId, DateTime? desde, DateTime? hasta, int? pageIndex, int? itemsPorPagina, CancellationToken ct)
    {
        ViewBag.Productos = new SelectList(await db.Productos.AsNoTracking().OrderBy(x => x.Nombre).ToListAsync(ct), "ProductosId", "Nombre", productoId);
        ViewBag.ProductoId = productoId; ViewBag.Desde = desde?.ToString("yyyy-MM-dd"); ViewBag.Hasta = hasta?.ToString("yyyy-MM-dd");
        if (desde.HasValue && hasta.HasValue && desde.Value.Date > hasta.Value.Date) ModelState.AddModelError(string.Empty, "La fecha inicial no puede ser posterior a la fecha final.");
        var resultado = ModelState.IsValid
            ? await reportes.ObtenerKardexPaginadoAsync(productoId, desde, hasta, pageIndex, itemsPorPagina, ct)
            : new Paginar<SimpleInventario.Web.ViewModels.KardexItemViewModel>(new List<SimpleInventario.Web.ViewModels.KardexItemViewModel>(), 0, 1, Math.Clamp(itemsPorPagina ?? 10, 1, 100));
        return View(resultado);
    }
    public async Task<IActionResult> KardexCsv(int? productoId, DateTime? desde, DateTime? hasta, CancellationToken ct)
    {
        var rows = await reportes.ObtenerKardexAsync(productoId, desde, hasta, ct);
        var sb = new StringBuilder("Folio,Nombre,Unidad,Precio,Cantidad,TotalImporte,Tipo,Fecha\r\n");
        static string Q(string s) => "\"" + s.Replace("\"", "\"\"") + "\"";
        foreach (var x in rows)
            sb.AppendLine(string.Join(",", new[] { x.Folio.ToString(CultureInfo.InvariantCulture), Q(x.Nombre), Q(x.Unidad), x.Precio.ToString(CultureInfo.InvariantCulture), x.Cantidad.ToString(CultureInfo.InvariantCulture), x.TotalImporte.ToString(CultureInfo.InvariantCulture), Q(x.Tipo), Q(x.Fecha.ToString("yyyy-MM-dd HH:mm:ss")) }));
        return File(Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray(), "text/csv; charset=utf-8", "Kardex.csv");
    }
    public async Task<IActionResult> ExistenciasCsv(CancellationToken ct)
    {
        var rows = await db.Productos.AsNoTracking().Include(x => x.Unidades).Where(x => x.Existencia > 0).OrderBy(x => x.Nombre).ToListAsync(ct);
        var sb = new StringBuilder("ProductosId,Nombre,Unidad,Precio,Existencia\r\n");
        foreach (var x in rows) sb.AppendLine($"{x.ProductosId},\"{x.Nombre.Replace("\"", "\"\"")}\",\"{x.Unidades?.Unidad}\",{x.Precio.ToString(CultureInfo.InvariantCulture)},{x.Existencia.ToString(CultureInfo.InvariantCulture)}");
        return File(Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray(), "text/csv; charset=utf-8", "Existencias.csv");
    }
}
