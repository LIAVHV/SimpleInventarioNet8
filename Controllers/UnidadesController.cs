using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleInventario.Web.Data;
using SimpleInventario.Web.Models;

namespace SimpleInventario.Web.Controllers;

public class UnidadesController(ApplicationDbContext db, ILogger<UnidadesController> logger) : Controller
{
    public async Task<IActionResult> Index(string? q, int? pageIndex, int? itemsPorPagina, CancellationToken ct)
    {
        var query = db.Unidades.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(x => x.Unidad.Contains(q.Trim()));

        ViewBag.Query = q;

        // Se utiliza PaginarEntidad pasando la consulta ordenada y los parámetros de página con valores por defecto (1 y 10)
        var resPag = await Paginar<Unidades>.PaginarEntidad(query.OrderBy(x => x.Unidad), pageIndex ?? 1, itemsPorPagina ?? 10);

        return View(resPag);
    }
    public IActionResult Create() => View(new Unidades());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Unidad")] Unidades model, CancellationToken ct)
    {
        model.Unidad = (model.Unidad ?? "").Trim().ToUpperInvariant();
        if (await db.Unidades.AnyAsync(x => x.Unidad == model.Unidad, ct)) ModelState.AddModelError(nameof(model.Unidad), "La unidad ya existe.");
        if (!ModelState.IsValid) return View(model);
        db.Unidades.Add(model);
        try
        {
            await db.SaveChangesAsync(ct);
            TempData["Success"] = "Unidad registrada.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException ex)
        {
            logger.LogWarning(ex, "Unidad duplicada o restricción al guardar.");
            ModelState.AddModelError(nameof(model.Unidad), "No fue posible guardar la unidad; verifica que no esté duplicada.");
            return View(model);
        }
    }
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var item = await db.Unidades.FindAsync(new object?[] { id }, ct); return item is null ? NotFound() : View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("UnidadesId,Unidad")] Unidades model, CancellationToken ct)
    {
        if (id != model.UnidadesId) return BadRequest();
        // El formulario original de modificación solo recortaba espacios; la normalización a mayúsculas se aplica al alta.
        model.Unidad = (model.Unidad ?? "").Trim();
        if (await db.Unidades.AnyAsync(x => x.UnidadesId != id && x.Unidad == model.Unidad, ct)) ModelState.AddModelError(nameof(model.Unidad), "La unidad ya existe.");
        if (!ModelState.IsValid) return View(model);
        var entity = await db.Unidades.FindAsync(new object?[] { id }, ct); if (entity is null) return NotFound();
        entity.Unidad = model.Unidad;
        try
        {
            await db.SaveChangesAsync(ct);
            TempData["Success"] = "Unidad actualizada.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException ex)
        {
            logger.LogWarning(ex, "No se pudo actualizar unidad {Id}", id);
            ModelState.AddModelError(nameof(model.Unidad), "La unidad ya existe o está relacionada.");
            return View(model);
        }
    }
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var item = await db.Unidades.AsNoTracking().Include(x => x.Productos).FirstOrDefaultAsync(x => x.UnidadesId == id, ct);
        return item is null ? NotFound() : View(item);
    }
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        var item = await db.Unidades.FindAsync(new object?[] { id }, ct);
        if (item is null) return NotFound();
        if (await db.Productos.AnyAsync(x => x.FkUnidadesId == id, ct))
        {
            TempData["Error"] = "No se puede eliminar: existen productos asociados a esta unidad.";
            return RedirectToAction(nameof(Index));
        }
        db.Unidades.Remove(item);
        await db.SaveChangesAsync(ct);
        TempData["Success"] = "Unidad eliminada.";
        return RedirectToAction(nameof(Index));
    }
}
