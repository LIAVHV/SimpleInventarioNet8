using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SimpleInventario.Web.Data;
using SimpleInventario.Web.Services;
using SimpleInventario.Web.Models;
using SimpleInventario.Web.ViewModels;

namespace SimpleInventario.Web.Controllers;

public class SalidasController(ApplicationDbContext db, IInventarioService inventario, ILogger<SalidasController> logger) : Controller
{
    private async Task CargarProductos(int? selected, CancellationToken ct)
    {
        ViewBag.Productos = new SelectList(
            await db.Productos
                .AsNoTracking()
                .Where(x => x.Existencia > 0)
                .OrderBy(x => x.Nombre)
                .ToListAsync(ct),
            "ProductosId", "Nombre", selected);
    }

    public async Task<IActionResult> Index(int? pageIndex, int? itemsPorPagina, CancellationToken ct)
    {
        var query = db.Salidas.AsNoTracking().Include(x => x.Productos);
        var resultado = await Paginar<Salidas>.PaginarEntidad(query.OrderByDescending(x => x.Fecha).ThenByDescending(x => x.SalidasId), pageIndex, itemsPorPagina, ct);
        return View(resultado);
    }
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        await CargarProductos(null, ct); return View(new SalidaViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SalidaViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await CargarProductos(model.ProductoId, ct);
            return View(model);
        }
        try
        {
            var result = await inventario.RegistrarSalidaAsync(model, ct);
            if (!result.Ok)
            {
                ModelState.AddModelError(string.Empty, result.Mensaje);
                await CargarProductos(model.ProductoId, ct);
                return View(model);
            }
            TempData["Success"] = $"{result.Mensaje} Existencia actual: {result.Existencia:N2}.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "No se pudo registrar la salida");
            ModelState.AddModelError(string.Empty, "Ocurrió un error al registrar la salida. No se guardaron cambios parciales.");
            await CargarProductos(model.ProductoId, ct);
            return View(model);
        }
    }
}
