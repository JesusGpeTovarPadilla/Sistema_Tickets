using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sistema_Tickets.Models;

[Table("TICKETS")]
[Index("AsignadoAid", Name = "IX_TICKETS_AsignadoAID")]
[Index("Codigo", Name = "IX_TICKETS_Codigo")]
[Index("EstadoId", Name = "IX_TICKETS_EstadoID")]
[Index("FechaCreacion", Name = "IX_TICKETS_FechaCreacion")]
[Index("Codigo", Name = "UQ__TICKETS__06370DAC906C6502", IsUnique = true)]
public partial class Ticket
{
    [Key]
    [Column("TicketID")]
    public int TicketId { get; set; }

    [StringLength(50)]
    public string Codigo { get; set; } = null!;

    [StringLength(200)]
    public string Asunto { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    [Column("EstadoID")]
    public int EstadoId { get; set; }

    [Column("PrioridadID")]
    public int PrioridadId { get; set; }

    [Column("CategoriaID")]
    public int CategoriaId { get; set; }

    [Column("CanalID")]
    public int CanalId { get; set; }

    [Column("CreadorID")]
    public int CreadorId { get; set; }

    [Column("AsignadoAID")]
    public int? AsignadoAid { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaCierre { get; set; }

    public double? DuracionResolucion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UltimaActualizacion { get; set; }

    [InverseProperty("Ticket")]
    public virtual ICollection<Adjunto> Adjuntos { get; set; } = new List<Adjunto>();

    [ForeignKey("AsignadoAid")]
    [InverseProperty("TicketAsignadoAs")]
    public virtual Usuario? AsignadoA { get; set; }

    [ForeignKey("CanalId")]
    [InverseProperty("Tickets")]
    public virtual Canale Canal { get; set; } = null!;

    [ForeignKey("CategoriaId")]
    [InverseProperty("Tickets")]
    public virtual Categoria Categoria { get; set; } = null!;

    [InverseProperty("Ticket")]
    public virtual ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();

    [ForeignKey("CreadorId")]
    [InverseProperty("TicketCreadors")]
    public virtual Usuario Creador { get; set; } = null!;

    [ForeignKey("EstadoId")]
    [InverseProperty("Tickets")]
    public virtual Estado Estado { get; set; } = null!;

    [ForeignKey("PrioridadId")]
    [InverseProperty("Tickets")]
    public virtual Prioridade Prioridad { get; set; } = null!;

    [ForeignKey("TicketId")]
    [InverseProperty("Tickets")]
    public virtual ICollection<Etiqueta> Etiqueta { get; set; } = new List<Etiqueta>();
}
