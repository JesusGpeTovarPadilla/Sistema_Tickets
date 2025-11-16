using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sistema_Tickets.Models;

[Table("ADJUNTOS")]
[Index("TicketId", Name = "IX_ADJUNTOS_TicketID")]
public partial class Adjunto
{
    [Key]
    [Column("AdjuntoID")]
    public int AdjuntoId { get; set; }

    [Column("TicketID")]
    public int TicketId { get; set; }

    [Column("ComentarioID")]
    public int? ComentarioId { get; set; }

    [StringLength(255)]
    public string Ruta { get; set; } = null!;

    [StringLength(255)]
    public string NombreOriginal { get; set; } = null!;

    public long TamanoBytes { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaSubida { get; set; }

    [ForeignKey("ComentarioId")]
    [InverseProperty("Adjuntos")]
    public virtual Comentario? Comentario { get; set; }

    [ForeignKey("TicketId")]
    [InverseProperty("Adjuntos")]
    public virtual Ticket Ticket { get; set; } = null!;
}
