using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sistema_Tickets.Models;

[Table("USUARIOS")]
[Index("Correo", Name = "UQ__USUARIOS__60695A1943EDA60F", IsUnique = true)]
public partial class Usuario
{
    [Key]
    [Column("UsuarioID")]
    public int UsuarioId { get; set; }

    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [StringLength(150)]
    public string Correo { get; set; } = null!;

    [StringLength(255)]
    public string PasswordHash { get; set; } = null!;

    [Column("RolID")]
    public int RolId { get; set; }

    [Column("DepartamentoID")]
    public int? DepartamentoId { get; set; }

    public bool Estado { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaAlta { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UltimaSeccion { get; set; }

    public bool Esbloqueado { get; set; }

    [StringLength(200)]
    public string? Imagen { get; set; }

    [InverseProperty("Usuario")]
    public virtual ICollection<Auditorium> Auditoria { get; set; } = new List<Auditorium>();

    [InverseProperty("Usuario")]
    public virtual ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();

    [ForeignKey("DepartamentoId")]
    [InverseProperty("Usuarios")]
    public virtual Departamento? Departamento { get; set; }

    [ForeignKey("RolId")]
    [InverseProperty("Usuarios")]
    public virtual Role Rol { get; set; } = null!;

    [InverseProperty("AsignadoA")]
    public virtual ICollection<Ticket> TicketAsignadoAs { get; set; } = new List<Ticket>();

    [InverseProperty("Creador")]
    public virtual ICollection<Ticket> TicketCreadors { get; set; } = new List<Ticket>();
}
