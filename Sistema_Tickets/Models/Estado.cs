using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sistema_Tickets.Models;

[Table("ESTADOS")]
public partial class Estado
{
    [Key]
    [Column("EstadoID")]
    public int EstadoId { get; set; }

    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    public int Orden { get; set; }

    public bool EsFinal { get; set; }

    public bool EsPausa { get; set; }

    [InverseProperty("Estado")]
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
