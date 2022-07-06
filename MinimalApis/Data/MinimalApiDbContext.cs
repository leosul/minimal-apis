using Microsoft.EntityFrameworkCore;
using MinimalApis.Models;

namespace MinimalApis.Data;

public class MinimalApiDbContext : DbContext
{
    public MinimalApiDbContext(DbContextOptions<MinimalApiDbContext> options) : base(options) { }
    public DbSet<Cliente> Clientes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>()
            .HasKey(s => s.Id);

        modelBuilder.Entity<Cliente>()
            .Property(s => s.Nome)
            .IsRequired()
            .HasColumnType("varchar(200)");

        modelBuilder.Entity<Cliente>()
            .Property(s => s.Nif)
            .IsRequired()
            .HasColumnType("varchar(20)");

        modelBuilder.Entity<Cliente>()
            .Property(s => s.Ativo)
            .IsRequired()
            .HasColumnType("bit");

        modelBuilder.Entity<Cliente>()
            .ToTable("Clientes");

        base.OnModelCreating(modelBuilder);
    }
}
