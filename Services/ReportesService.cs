using Microsoft.EntityFrameworkCore;
using SimpleInventario.Web.Data;
using SimpleInventario.Web.Models;
using SimpleInventario.Web.ViewModels;

namespace SimpleInventario.Web.Services;

public class ReportesService(ApplicationDbContext db) : IReportesService
{
    private IQueryable<KardexItemViewModel> CrearConsultaKardex(int? productoId, DateTime? desde, DateTime? hasta)
    {
        var entradas = db.Entradas.AsNoTracking().Where(x => !productoId.HasValue || x.FKProductosId == productoId)
            .Where(x => !desde.HasValue || x.Fecha >= desde.Value)
            .Where(x => !hasta.HasValue || x.Fecha < hasta.Value.Date.AddDays(1))
            .Select(x => new KardexItemViewModel
            {
                Folio = x.EntradasId,
                Nombre = x.Productos!.Nombre,
                Unidad = x.Productos.Unidades!.Unidad,
                Precio = x.Precio,
                Cantidad = x.Cantidad,
                TotalImporte = x.TotalImporte,
                Tipo = TipoMovimiento.Entrada.ToString(),
                Fecha = x.Fecha
            });
        var salidas = db.Salidas.AsNoTracking().Where(x => !productoId.HasValue || x.FKProductosId == productoId)
            .Where(x => !desde.HasValue || x.Fecha >= desde.Value)
            .Where(x => !hasta.HasValue || x.Fecha < hasta.Value.Date.AddDays(1))
            .Select(x => new KardexItemViewModel
            {
                Folio = x.SalidasId,
                Nombre = x.Productos!.Nombre,
                Unidad = x.Productos.Unidades!.Unidad,
                Precio = x.Precio,
                Cantidad = x.Cantidad * -1m,
                TotalImporte = x.TotalImporte,
                Tipo = TipoMovimiento.Salida.ToString(),
                Fecha = x.Fecha
            });
        return entradas.Union(salidas).OrderBy(x => x.Nombre).ThenBy(x => x.Fecha).ThenBy(x => x.Tipo);
    }

    public Task<List<KardexItemViewModel>> ObtenerKardexAsync(int? productoId, DateTime? desde, DateTime? hasta, CancellationToken ct = default)
        => CrearConsultaKardex(productoId, desde, hasta).ToListAsync(ct);

    public Task<Paginar<KardexItemViewModel>> ObtenerKardexPaginadoAsync(int? productoId, DateTime? desde, DateTime? hasta, int? pageIndex, int? itemsPorPagina, CancellationToken ct = default)
        => Paginar<KardexItemViewModel>.PaginarEntidad(CrearConsultaKardex(productoId, desde, hasta), pageIndex, itemsPorPagina, ct);
}
