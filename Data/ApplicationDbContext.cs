using Microsoft.EntityFrameworkCore;
using SimpleInventario.Web.Models;

namespace SimpleInventario.Web.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Unidades> Unidades => Set<Unidades>();
    public DbSet<Productos> Productos => Set<Productos>();
    public DbSet<Entradas> Entradas => Set<Entradas>();
    public DbSet<Salidas> Salidas => Set<Salidas>();
    public DbSet<Proveedores> Proveedores => Set<Proveedores>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Unidades>().HasIndex(x => x.Unidad).IsUnique();
        modelBuilder.Entity<Productos>().HasIndex(x => x.Nombre).IsUnique();
        modelBuilder.Entity<Productos>().Property(x => x.Precio).HasPrecision(18, 2);
        modelBuilder.Entity<Productos>().Property(x => x.Existencia).HasPrecision(18, 2);
        modelBuilder.Entity<Productos>().Property(x => x.StockMinimo).HasPrecision(18, 2);
        modelBuilder.Entity<Productos>().Property(x => x.PrecioCompra).HasPrecision(18, 2);
        modelBuilder.Entity<Proveedores>().HasIndex(x => x.Nombre).IsUnique();
        modelBuilder.Entity<Entradas>().HasOne(x => x.Proveedores).WithMany(x => x.Entradas).HasForeignKey(x => x.FKProveedorId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Entradas>().Property(x => x.Precio).HasPrecision(18, 2);
        modelBuilder.Entity<Entradas>().Property(x => x.PrecioVenta).HasPrecision(18, 2);
        modelBuilder.Entity<Entradas>().Property(x => x.Cantidad).HasPrecision(18, 2);
        modelBuilder.Entity<Entradas>().Property(x => x.TotalImporte).HasPrecision(18, 2);
        modelBuilder.Entity<Salidas>().Property(x => x.Precio).HasPrecision(18, 2);
        modelBuilder.Entity<Salidas>().Property(x => x.Cantidad).HasPrecision(18, 2);
        modelBuilder.Entity<Salidas>().Property(x => x.TotalImporte).HasPrecision(18, 2);
        modelBuilder.Entity<Productos>().HasOne(x => x.Unidades).WithMany(x => x.Productos)
            .HasForeignKey(x => x.FkUnidadesId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Entradas>().HasOne(x => x.Productos).WithMany(x => x.Entradas)
            .HasForeignKey(x => x.FKProductosId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Salidas>().HasOne(x => x.Productos).WithMany(x => x.Salidas)
            .HasForeignKey(x => x.FKProductosId).OnDelete(DeleteBehavior.Restrict);

    }
}
