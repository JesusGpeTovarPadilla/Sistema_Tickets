using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sistema_Tickets.Models;

[Table("ETIQUETAS")]
[Index("Nombre", Name = "UQ__ETIQUETA__75E3EFCFF2D54381", IsUnique = true)]
public partial class Etiqueta
{
    [Key]
    [Column("EtiquetaID")]
    public int EtiquetaId { get; set; }

    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [ForeignKey("EtiquetaId")]
    [InverseProperty("Etiqueta")]
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
