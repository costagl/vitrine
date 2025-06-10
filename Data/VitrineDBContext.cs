using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VitrineApi.Models;

namespace VitrineApi.Data;

public partial class VitrineDBContext : IdentityDbContext<LojistaAuth>
{
    public VitrineDBContext(DbContextOptions<VitrineDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CategoriaProduto> CategoriaProduto { get; set; }

    public virtual DbSet<Cliente> Cliente { get; set; }

    public virtual DbSet<EnderecoEntrega> EnderecoEntrega { get; set; }

    public virtual DbSet<ItensPedido> ItensPedido { get; set; }

    public virtual DbSet<Layout> Layout { get; set; }

    public virtual DbSet<Loja> Loja { get; set; }

    public virtual DbSet<Lojista> Lojista { get; set; }

    public virtual DbSet<Pedido> Pedido { get; set; }

    public virtual DbSet<Produto> Produto { get; set; }

    public virtual DbSet<Tema> Tema { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EnderecoEntrega>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.EnderecoEntrega).HasConstraintName("FK_EnderecoEntrega_Cliente");
        });

        modelBuilder.Entity<ItensPedido>(entity =>
        {
            entity.HasOne(d => d.IdPedidoNavigation).WithMany(p => p.ItensPedido)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ItensPedido_Pedido");

            entity.HasOne(d => d.IdProdutoNavigation).WithMany(p => p.ItensPedido)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ItensPedido_Produto");
        });

        modelBuilder.Entity<Loja>(entity =>
        {
            entity.Property(e => e.Subdominio).HasDefaultValue("");

            entity.HasOne(d => d.CpfNavigation).WithMany(p => p.Loja)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Loja_Lojista");

            entity.HasOne(d => d.IdLayoutNavigation).WithMany(p => p.Loja)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Loja_Layout");

            entity.HasOne(d => d.IdTemaNavigation).WithMany(p => p.Loja)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Loja_Tema");
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Pedido)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pedido_Cliente");

            entity.HasOne(d => d.IdEnderecoEntregaNavigation).WithMany(p => p.Pedido)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pedido_EnderecoEntrega");
        });

        modelBuilder.Entity<Produto>(entity =>
        {
            entity.HasOne(d => d.IdCategoriaProdutoNavigation).WithMany(p => p.Produto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Produto_CategoriaProduto");

            entity.HasOne(d => d.IdLojaNavigation).WithMany(p => p.Produto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Produto_Loja");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}