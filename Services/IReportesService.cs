using SimpleInventario.Web.Models;
using SimpleInventario.Web.ViewModels;

namespace SimpleInventario.Web.Services;

public interface IReportesService
{
    Task<List<KardexItemViewModel>> ObtenerKardexAsync(int? productoId, DateTime? desde, DateTime? hasta, CancellationToken ct = default);
    Task<Paginar<KardexItemViewModel>> ObtenerKardexPaginadoAsync(int? productoId, DateTime? desde, DateTime? hasta, int? pageIndex, int? itemsPorPagina, CancellationToken ct = default);
}
