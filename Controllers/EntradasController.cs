using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SimpleInventario.Web.Data;
using SimpleInventario.Web.Models;
using SimpleInventario.Web.Services;
using SimpleInventario.Web.ViewModels;

namespace SimpleInventario.Web.Controllers;

public class EntradasController(ApplicationDbContext db, IInventarioService inventario, ILogger<EntradasController> logger) : Controller
{
    private async Task CargarProductos(int? selected, CancellationToken ct)
    {
        ViewBag.Productos = new SelectList(await db.Productos.AsNoTracking().OrderBy(x => x.Nombre).ToListAsync(ct), "ProductosId", "Nombre", selected);
    }

    public async Task<IActionResult> Index(int? pageIndex, int? itemsPorPagina, CancellationToken ct)
    {
        var query = db.Entradas.AsNoTracking().Include(x => x.Productos).Include(x=>x.Proveedores);
        var resultado = await Paginar<Entradas>.PaginarEntidad(query.OrderByDescending(x => x.Fecha).ThenByDescending(x => x.EntradasId), pageIndex ?? 1, itemsPorPagina ?? 10, ct);
        return View(resultado);
    }
   

    public async Task<IActionResult> Create(CancellationToken ct)
    {
        await CargarProductos(null, ct);
        await CargarProveedores(null, ct);

        return View(new EntradaViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EntradaViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) {
            await CargarProductos(model.ProductoId, ct);
            await CargarProveedores(model.ProveedorId, ct);

            return View(model); }
        try
        {
            var result = await inventario.RegistrarEntradaAsync(model, ct);
            if (!result.Ok)
            {
                ModelState.AddModelError(string.Empty, result.Mensaje);
                await CargarProductos(model.ProductoId, ct);
                await CargarProveedores(model.ProveedorId, ct);

                return View(model);
            }
            TempData["Success"] = $"{result.Mensaje} Existencia actual: {result.Existencia:N2}."; return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) { logger.LogError(ex, "No se pudo registrar la entrada"); ModelState.AddModelError(string.Empty, "Ocurrió un error al registrar la entrada. No se guardaron cambios parciales."); await CargarProductos(model.ProductoId, ct); return View(model); }
    }

    private async Task CargarProveedores(int? selected, CancellationToken ct)
    {
        ViewBag.Proveedores = new SelectList(
            await db.Proveedores
                .AsNoTracking()
                .Where(x => x.Nombre != "PROVEEDOR HISTORICO")
                .OrderBy(x => x.Nombre)
                .ToListAsync(ct), "ProveedoresId", "Nombre", selected);
    }

}
