using Microsoft.EntityFrameworkCore;
using GestaPME.Models;

namespace GestaPME.Data;

public class AppDbContext : DbContext{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){
    }

    public DbSet<Empresa> Empresas => Set<Empresa>();
    public DbSet<Departamento> Departamentos => Set<Departamento>();
    public DbSet<Cargo> Cargos => Set<Cargo>();
    public DbSet<Funcionario> Funcionarios => Set<Funcionario>();
    public DbSet<Ferias> Ferias => Set<Ferias>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder){
        // Empresa
        modelBuilder.Entity<Empresa>(e => { e.HasIndex(x => x.CNPJ).IsUnique(); });

        // Departamento
        modelBuilder.Entity<Departamento>(e => {
            e.HasOne(d => d.Empresa)
                .WithMany(emp => emp.Departamentos)
                .HasForeignKey(d => d.EmpresaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Cargo
        modelBuilder.Entity<Cargo>(e => {
            e.HasOne(c => c.Empresa)
                .WithMany(emp => emp.Cargos)
                .HasForeignKey(c => c.EmpresaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Funcionario
        modelBuilder.Entity<Funcionario>(e => {
            e.HasIndex(f => f.CPF).IsUnique();

            e.HasOne(f => f.Empresa)
                .WithMany(emp => emp.Funcionarios)
                .HasForeignKey(f => f.EmpresaId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(f => f.Departamento)
                .WithMany(d => d.Funcionarios)
                .HasForeignKey(f => f.DepartamentoId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(f => f.Cargo)
                .WithMany(c => c.Funcionarios)
                .HasForeignKey(f => f.CargoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Ferias
        modelBuilder.Entity<Ferias>(e => {
            e.HasOne(f => f.Funcionario)
                .WithMany(func => func.Ferias)
                .HasForeignKey(f => f.FuncionarioId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Usuario
        modelBuilder.Entity<Usuario>(e => {
            e.HasIndex(u => u.Email).IsUnique();

            e.HasOne(u => u.Empresa)
                .WithMany(emp => emp.Usuarios)
                .HasForeignKey(u => u.EmpresaId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    public override int SaveChanges(){
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default){
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps(){
        var entries = ChangeTracker.Entries<BaseEntity>();
        foreach (var entry in entries){
            if (entry.State == EntityState.Modified)
                entry.Entity.AtualizadoEm = DateTime.UtcNow;
        }
    }
}