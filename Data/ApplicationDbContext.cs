using ImportExportFiles.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ImportExportFiles.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.PartSku)
            .IsUnique();
    }
}
