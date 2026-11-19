
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleInventario.Web.Data;
using SimpleInventario.Web.Models;

namespace SimpleInventario.Web.Controllers;

public class ProveedoresController(
    ApplicationDbContext db,
    ILogger<ProveedoresController> logger) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var proveedores = await db.Proveedores
            .AsNoTracking()
            .OrderBy(x => x.Nombre)
            .ToListAsync(ct);

        return View(proveedores);
    }

    public IActionResult Create()
    {
        return View(new Proveedores());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        Proveedores model,
        CancellationToken ct)
    {
        model.Nombre = Normalizar(model.Nombre);
        model.Telefono = model.Telefono.Trim();

        if (await db.Proveedores.AnyAsync(
            x => x.Nombre == model.Nombre, ct))
        {
            ModelState.AddModelError(
                nameof(model.Nombre),
                "Ya existe un proveedor con ese nombre.");
        }

        if (!ModelState.IsValid)
            return View(model);

        db.Proveedores.Add(model);

        try
        {
            await db.SaveChangesAsync(ct);

            TempData["Success"] = "Proveedor registrado.";

            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Error al registrar proveedor.");

            ModelState.AddModelError(
                string.Empty,
                "No fue posible registrar el proveedor.");

            return View(model);
        }
    }

    public async Task<IActionResult> Edit(
        int id,
        CancellationToken ct)
    {
        var proveedor = await db.Proveedores.FindAsync(
            new object?[] { id }, ct);

        return proveedor is null ? NotFound() : View(proveedor);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        Proveedores model,
        CancellationToken ct)
    {
        if (id != model.ProveedoresId)
            return BadRequest();

        model.Nombre = Normalizar(model.Nombre);
        model.Telefono = model.Telefono.Trim();

        if (await db.Proveedores.AnyAsync(
            x => x.ProveedoresId != id &&
                 x.Nombre == model.Nombre, ct))
        {
            ModelState.AddModelError(
                nameof(model.Nombre),
                "Ya existe un proveedor con ese nombre.");
        }

        if (!ModelState.IsValid)
            return View(model);

        var entity = await db.Proveedores.FindAsync(
            new object?[] { id }, ct);

        if (entity is null)
            return NotFound();

        entity.Nombre = model.Nombre;
        entity.Telefono = model.Telefono;

        try
        {
            await db.SaveChangesAsync(ct);

            TempData["Success"] = "Proveedor actualizado.";

            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Error al actualizar proveedor {Id}", id);

            ModelState.AddModelError(
                string.Empty,
                "No fue posible actualizar el proveedor.");

            return View(model);
        }
    }

    public async Task<IActionResult> Delete(
        int id,
        CancellationToken ct)
    {
        var proveedor = await db.Proveedores
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ProveedoresId == id, ct);

        return proveedor is null ? NotFound() : View(proveedor);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(
        int id,
        CancellationToken ct)
    {
        var proveedor = await db.Proveedores.FindAsync(
            new object?[] { id }, ct);

        if (proveedor is null)
            return NotFound();

        if (await db.Entradas.AnyAsync(
            x => x.FKProveedorId == id, ct))
        {
            TempData["Error"] =
                "No se puede eliminar: el proveedor tiene entradas registradas.";

            return RedirectToAction(nameof(Index));
        }

        db.Proveedores.Remove(proveedor);

        await db.SaveChangesAsync(ct);

        TempData["Success"] = "Proveedor eliminado.";

        return RedirectToAction(nameof(Index));
    }

    private static string Normalizar(string? value) =>
        string.Join(' ',
            (value ?? "").Trim().ToUpperInvariant()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries));
}
