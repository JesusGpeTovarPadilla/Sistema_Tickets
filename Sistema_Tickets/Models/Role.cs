using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sistema_Tickets.Models;

[Table("ROLES")]
public partial class Role
{
    [Key]
    [Column("RolID")]
    public int RolId { get; set; }

    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [StringLength(255)]
    public string? Descripcion { get; set; }

    public bool Estatus { get; set; }

    [InverseProperty("Rol")]
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
