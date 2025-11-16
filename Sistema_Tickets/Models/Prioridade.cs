using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sistema_Tickets.Models;

[Table("PRIORIDADES")]
public partial class Prioridade
{
    [Key]
    [Column("PrioridadID")]
    public int PrioridadId { get; set; }

    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    public int Peso { get; set; }

    [InverseProperty("Prioridad")]
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
