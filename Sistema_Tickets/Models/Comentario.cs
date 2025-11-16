using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sistema_Tickets.Models;

[Table("COMENTARIOS")]
[Index("TicketId", Name = "IX_COMENTARIOS_TicketID")]
public partial class Comentario
{
    [Key]
    [Column("ComentarioID")]
    public int ComentarioId { get; set; }

    [Column("TicketID")]
    public int TicketId { get; set; }

    [Column("UsuarioID")]
    public int UsuarioId { get; set; }

    public string Mensaje { get; set; } = null!;

    public bool EsInterno { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Fecha { get; set; }

    [InverseProperty("Comentario")]
    public virtual ICollection<Adjunto> Adjuntos { get; set; } = new List<Adjunto>();

    [ForeignKey("TicketId")]
    [InverseProperty("Comentarios")]
    public virtual Ticket Ticket { get; set; } = null!;

    [ForeignKey("UsuarioId")]
    [InverseProperty("Comentarios")]
    public virtual Usuario Usuario { get; set; } = null!;
}
