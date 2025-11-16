using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Sistema_Tickets.Models;

namespace Sistema_Tickets.Data;

public partial class TicketSystemDbContext : DbContext
{
    public TicketSystemDbContext()
    {
    }

    public TicketSystemDbContext(DbContextOptions<TicketSystemDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Adjunto> Adjuntos { get; set; }

    public virtual DbSet<Auditorium> Auditoria { get; set; }

    public virtual DbSet<Canale> Canales { get; set; }

    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<Comentario> Comentarios { get; set; }

    public virtual DbSet<Configuracionempresa> Configuracionempresas { get; set; }

    public virtual DbSet<Departamento> Departamentos { get; set; }

    public virtual DbSet<Estado> Estados { get; set; }

    public virtual DbSet<Etiqueta> Etiquetas { get; set; }

    public virtual DbSet<Prioridade> Prioridades { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=TicketSystemDB;Trusted_Connection=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Adjunto>(entity =>
        {
            entity.HasKey(e => e.AdjuntoId).HasName("PK__ADJUNTOS__2ECBD560B925AD7D");

            entity.Property(e => e.FechaSubida).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Comentario).WithMany(p => p.Adjuntos).HasConstraintName("FK__ADJUNTOS__Coment__4CA06362");

            entity.HasOne(d => d.Ticket).WithMany(p => p.Adjuntos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ADJUNTOS__Ticket__4BAC3F29");
        });

        modelBuilder.Entity<Auditorium>(entity =>
        {
            entity.HasKey(e => e.AuditoriaId).HasName("PK__AUDITORI__095694E3DB81C096");

            entity.Property(e => e.Fecha).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Auditoria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AUDITORIA__Usuar__571DF1D5");
        });

        modelBuilder.Entity<Canale>(entity =>
        {
            entity.HasKey(e => e.CanalId).HasName("PK__CANALES__32B165770F0637EF");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.CategoriaId).HasName("PK__CATEGORI__F353C1C522DF4A46");

            entity.HasOne(d => d.CategoriaPadre).WithMany(p => p.InverseCategoriaPadre).HasConstraintName("FK__CATEGORIA__Categ__31EC6D26");
        });

        modelBuilder.Entity<Comentario>(entity =>
        {
            entity.HasKey(e => e.ComentarioId).HasName("PK__COMENTAR__F184495818BF8F32");

            entity.Property(e => e.Fecha).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Ticket).WithMany(p => p.Comentarios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__COMENTARI__Ticke__46E78A0C");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Comentarios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__COMENTARI__Usuar__47DBAE45");
        });

        modelBuilder.Entity<Configuracionempresa>(entity =>
        {
            entity.HasKey(e => e.ConfigId).HasName("PK__CONFIGUR__C3BC333C702CEF03");

            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<Departamento>(entity =>
        {
            entity.HasKey(e => e.DepartamentoId).HasName("PK__DEPARTAM__66BB0E1E60679F1F");
            entity.Property(e => e.Estatus).HasDefaultValue(true);
        });

        modelBuilder.Entity<Estado>(entity =>
        {
            entity.HasKey(e => e.EstadoId).HasName("PK__ESTADOS__FEF86B6036D028C7");
        });

        modelBuilder.Entity<Etiqueta>(entity =>
        {
            entity.HasKey(e => e.EtiquetaId).HasName("PK__ETIQUETA__3C637833969C30C9");
        });

        modelBuilder.Entity<Prioridade>(entity =>
        {
            entity.HasKey(e => e.PrioridadId).HasName("PK__PRIORIDA__393917CE86763D31");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RolId).HasName("PK__ROLES__F92302D12EB3DBC0");
            entity.Property(e => e.Estatus).HasDefaultValue(true);
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketId).HasName("PK__TICKETS__712CC627658ECD59");

            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UltimaActualizacion).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.AsignadoA).WithMany(p => p.TicketAsignadoAs).HasConstraintName("FK__TICKETS__Asignad__4222D4EF");

            entity.HasOne(d => d.Canal).WithMany(p => p.Tickets)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TICKETS__CanalID__403A8C7D");

            entity.HasOne(d => d.Categoria).WithMany(p => p.Tickets)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TICKETS__Categor__3F466844");

            entity.HasOne(d => d.Creador).WithMany(p => p.TicketCreadors)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TICKETS__Creador__412EB0B6");

            entity.HasOne(d => d.Estado).WithMany(p => p.Tickets)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TICKETS__EstadoI__3D5E1FD2");

            entity.HasOne(d => d.Prioridad).WithMany(p => p.Tickets)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TICKETS__Priorid__3E52440B");

            entity.HasMany(d => d.Etiqueta).WithMany(p => p.Tickets)
                .UsingEntity<Dictionary<string, object>>(
                    "TicketEtiqueta",
                    r => r.HasOne<Etiqueta>().WithMany()
                        .HasForeignKey("EtiquetaId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__TICKET_ET__Etiqu__534D60F1"),
                    l => l.HasOne<Ticket>().WithMany()
                        .HasForeignKey("TicketId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__TICKET_ET__Ticke__52593CB8"),
                    j =>
                    {
                        j.HasKey("TicketId", "EtiquetaId").HasName("PK__TICKET_E__52EAF1A4A5D179BC");
                        j.ToTable("TICKET_ETIQUETAS");
                        j.IndexerProperty<int>("TicketId").HasColumnName("TicketID");
                        j.IndexerProperty<int>("EtiquetaId").HasColumnName("EtiquetaID");
                    });
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.UsuarioId).HasName("PK__USUARIOS__2B3DE7985548F835");

            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.FechaAlta).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Departamento).WithMany(p => p.Usuarios).HasConstraintName("FK__USUARIOS__Depart__2D27B809");

            entity.HasOne(d => d.Rol).WithMany(p => p.Usuarios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__USUARIOS__RolID__2C3393D0");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
