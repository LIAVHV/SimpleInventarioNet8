using Microsoft.EntityFrameworkCore;
using SimpleInventario.Web.Data;
using SimpleInventario.Web.Models;
using SimpleInventario.Web.ViewModels;

namespace SimpleInventario.Web.Services;

public class InventarioService(ApplicationDbContext db, ILogger<InventarioService> logger) : IInventarioService
{

    public async Task<(bool Ok, string Mensaje, decimal Existencia)> RegistrarEntradaAsync(EntradaViewModel model, CancellationToken ct = default)
    {
        if (model.Precio <= 0 || model.PrecioVenta <= 0 || model.Cantidad <= 0)
        {
            return (false, "Los precios y la cantidad deben ser mayores que cero.", 0);
        }

        if (!await db.Proveedores.AnyAsync(
            x => x.ProveedoresId == model.ProveedorId &&
                 x.Nombre != "PROVEEDOR HISTORICO", ct))
        {
            return (false, "Selecciona un proveedor válido.", 0);
        }

        await using var tx = await db.Database.BeginTransactionAsync(ct);
        try
        {
            var affected = await db.Database.ExecuteSqlInterpolatedAsync($@"
            UPDATE dbo.Productos
            SET Existencia = Existencia + {model.Cantidad},
                PrecioCompra = {model.Precio},
                Precio = {model.PrecioVenta}
            WHERE ProductosId = {model.ProductoId};", ct);

            if (affected == 0)
            {
                await tx.RollbackAsync(ct);
                return (false, "El producto no existe.", 0);
            }

            var existencia = await db.Productos
                .AsNoTracking()
                .Where(x => x.ProductosId == model.ProductoId)
                .Select(x => x.Existencia)
                .SingleAsync(ct);

            db.Entradas.Add(new Entradas
            {
                FKProductosId = model.ProductoId,
                FKProveedorId = model.ProveedorId,
                Precio = model.Precio,
                PrecioVenta = model.PrecioVenta,
                Cantidad = model.Cantidad,
                TotalImporte = model.Precio * model.Cantidad,
                Fecha = DateTime.Now
            });

            await db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return (true, "La entrada se registró correctamente.", existencia);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync(ct);
            logger.LogError(ex, "Error al registrar entrada para producto {ProductoId}", model.ProductoId);
            throw;
        }
    }

    public async Task<(bool Ok, string Mensaje, decimal Existencia)> RegistrarSalidaAsync(SalidaViewModel model, CancellationToken ct = default)
    {
        if (model.Cantidad <= 0 || model.PrecioVenta <= 0)
            return (false, "El precio y la cantidad deben ser mayores que cero.", 0);

        await using var tx =
            await db.Database.BeginTransactionAsync(ct);

        try
        {
            var affected = await db.Database.ExecuteSqlInterpolatedAsync($@"
            UPDATE dbo.Productos
            SET Existencia = Existencia - {model.Cantidad}
            WHERE ProductosId = {model.ProductoId}
              AND Existencia >= {model.Cantidad};", ct);

            if (affected == 0)
            {
                var exists = await db.Productos.AnyAsync(
                    x => x.ProductosId == model.ProductoId, ct);

                await tx.RollbackAsync(ct);

                return ( false, exists ? "No existe stock suficiente para realizar la salida." : "El producto seleccionado no existe.", 0);
            }

            var existencia = await db.Productos
                .AsNoTracking()
                .Where(x => x.ProductosId == model.ProductoId)
                .Select(x => x.Existencia)
                .SingleAsync(ct);

            db.Salidas.Add(new Salidas
            {
                FKProductosId = model.ProductoId,
                Precio = model.PrecioVenta,
                Cantidad = model.Cantidad,
                TotalImporte = model.PrecioVenta * model.Cantidad * -1m,
                Fecha = DateTime.Now
            });

            await db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return (true, "La salida se registró correctamente.", existencia);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync(ct);
            logger.LogError(ex, "Error al registrar salida para producto {ProductoId}", model.ProductoId);
            throw;
        }
    }



}
