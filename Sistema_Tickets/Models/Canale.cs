using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sistema_Tickets.Models;

[Table("CANALES")]
public partial class Canale
{
    [Key]
    [Column("CanalID")]
    public int CanalId { get; set; }

    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [StringLength(255)]
    public string? Detalle { get; set; }

    [InverseProperty("Canal")]
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
