using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sistema_Tickets.Models;

[Table("DEPARTAMENTOS")]
public partial class Departamento
{
    [Key]
    [Column("DepartamentoID")]
    public int DepartamentoId { get; set; }

    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [StringLength(255)]
    public string? Descripcion { get; set; }

    [StringLength(150)]
    public string? Email { get; set; }

    public bool Estatus { get; set; }

    [InverseProperty("Departamento")]
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
