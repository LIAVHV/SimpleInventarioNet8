using SimpleInventario.Web.ViewModels;

namespace SimpleInventario.Web.Services;

public interface IInventarioService
{
    Task<(bool Ok, string Mensaje, decimal Existencia)> RegistrarEntradaAsync(EntradaViewModel model, CancellationToken ct = default);
    Task<(bool Ok, string Mensaje, decimal Existencia)> RegistrarSalidaAsync(SalidaViewModel model, CancellationToken ct = default);
}
