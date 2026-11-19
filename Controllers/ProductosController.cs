using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SimpleInventario.Web.Data;
using SimpleInventario.Web.Models;

namespace SimpleInventario.Web.Controllers;

public class ProductosController(ApplicationDbContext db, ILogger<ProductosController> logger) : Controller
{
    private async Task CargarUnidades(int? selected, CancellationToken ct)
    {
        ViewBag.Unidades = new SelectList(await db.Unidades.AsNoTracking().OrderBy(x => x.Unidad).ToListAsync(ct), "UnidadesId", "Unidad", selected);
    }
    public async Task<IActionResult> Index(string? q, int? pageIndex, int? itemsPorPagina, CancellationToken ct)
    {
        var query = db.Productos.AsNoTracking().Include(x => x.Unidades).AsQueryable();
        if (!string.IsNullOrWhiteSpace(q)) query = query.Where(x => x.Nombre.Contains(q.Trim()));
        ViewBag.Query = q;
        var resultado = await Paginar<Productos>.PaginarEntidad(query.OrderBy(x => x.Nombre), pageIndex, itemsPorPagina, ct);
        return View(resultado);
    }
    public async Task<IActionResult> Create(CancellationToken ct) { await CargarUnidades(null, ct); return View(new Productos()); }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nombre,FkUnidadesId,StockMinimo")] Productos model, CancellationToken ct)
    {
        model.Nombre = Normalizar(model.Nombre);
        model.Precio = 0;
        model.PrecioCompra = 0;
        model.Existencia = 0;

        if (!await db.Unidades.AnyAsync(x => x.UnidadesId == model.FkUnidadesId, ct)) ModelState.AddModelError(nameof(model.FkUnidadesId), "Selecciona una unidad válida.");
        if (await db.Productos.AnyAsync(x => x.Nombre == model.Nombre, ct)) ModelState.AddModelError(nameof(model.Nombre), "Ya existe un producto con ese nombre.");
        if (!ModelState.IsValid) { await CargarUnidades(model.FkUnidadesId, ct); return View(model); }
        db.Productos.Add(model);
        try { await db.SaveChangesAsync(ct); TempData["Success"] = "Producto registrado."; return RedirectToAction(nameof(Index)); }
        catch (DbUpdateException ex) { logger.LogWarning(ex, "Error al crear producto"); ModelState.AddModelError(nameof(model.Nombre), "No fue posible guardar; revisa que el nombre sea único."); await CargarUnidades(model.FkUnidadesId, ct); return View(model); }
    }
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var item = await db.Productos.FindAsync(new object?[] { id }, ct); if (item is null) return NotFound(); await CargarUnidades(item.FkUnidadesId, ct); return View(item);
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("ProductosId,Nombre,FkUnidadesId,StockMinimo")] Productos model, CancellationToken ct)
    {
        if (id != model.ProductosId) return BadRequest(); model.Nombre = Normalizar(model.Nombre);
        if (!await db.Unidades.AnyAsync(x => x.UnidadesId == model.FkUnidadesId, ct)) ModelState.AddModelError(nameof(model.FkUnidadesId), "Selecciona una unidad válida.");
        if (await db.Productos.AnyAsync(x => x.ProductosId != id && x.Nombre == model.Nombre, ct)) ModelState.AddModelError(nameof(model.Nombre), "Ya existe un producto con ese nombre.");
        if (!ModelState.IsValid) { await CargarUnidades(model.FkUnidadesId, ct); return View(model); }
        var entity = await db.Productos.FindAsync(new object?[] { id }, ct); if (entity is null) return NotFound();
        entity.Nombre = model.Nombre; 
        entity.FkUnidadesId = model.FkUnidadesId;
        entity.StockMinimo = model.StockMinimo;

        try { await db.SaveChangesAsync(ct); TempData["Success"] = "Producto actualizado."; return RedirectToAction(nameof(Index)); }
        catch (DbUpdateException ex) { logger.LogWarning(ex, "Error al editar producto {Id}", id); ModelState.AddModelError(nameof(model.Nombre), "No fue posible guardar; revisa el nombre."); await CargarUnidades(model.FkUnidadesId, ct); return View(model); }
    }
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var item = await db.Productos.AsNoTracking().Include(x => x.Unidades).FirstOrDefaultAsync(x => x.ProductosId == id, ct); return item is null ? NotFound() : View(item);
    }
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        var item = await db.Productos.FindAsync(new object?[] { id }, ct); if (item is null) return NotFound();
        if (await db.Entradas.AnyAsync(x => x.FKProductosId == id, ct) || await db.Salidas.AnyAsync(x => x.FKProductosId == id, ct))
        { TempData["Error"] = "No se puede eliminar: el producto tiene movimientos registrados."; return RedirectToAction(nameof(Index)); }
        db.Productos.Remove(item); await db.SaveChangesAsync(ct); TempData["Success"] = "Producto eliminado."; return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    [HttpGet]
    public async Task<IActionResult> Datos(int id,CancellationToken ct)
    {
        var producto = await db.Productos
            .AsNoTracking()
            .Where(x => x.ProductosId == id)
            .Select(x => new
            {
                x.PrecioCompra,
                PrecioVenta = x.Precio,
                x.Existencia
            })
            .FirstOrDefaultAsync(ct);

        return producto is null ? NotFound() : Json(producto);
    }


    public async Task<IActionResult> SinExistencia(string? q,int? pageIndex,int? itemsPorPagina,CancellationToken ct)
    {
        var query = db.Productos
            .AsNoTracking()
            .Include(p => p.Unidades)
            .Where(p => p.Existencia <= 0)
            .AsQueryable();

        // Filtro de búsqueda
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(p =>
                p.Nombre.Contains(q.Trim()));
        }

        ViewBag.Query = q;

        // Aplicar paginación
        var resultado = await Paginar<Productos>
            .PaginarEntidad(
                query.OrderBy(p => p.Nombre),
                pageIndex,
                itemsPorPagina,
                ct);

        return View(resultado);
    }

    public async Task<IActionResult> StockBajo(CancellationToken ct)
    {
        var productos = await db.Productos
            .AsNoTracking()
            .Include(x => x.Unidades)
            .Where(x =>
                x.StockMinimo > 0 &&
                x.Existencia <= x.StockMinimo)
            .OrderBy(x => x.Existencia)
            .ThenBy(x => x.Nombre)
            .ToListAsync(ct);

        return View(productos);
    }


    private static string Normalizar(string? value) => string.Join(' ', (value ?? "").Trim().ToUpperInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries));
}
