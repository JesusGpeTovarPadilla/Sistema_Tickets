using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sistema_Tickets.Models;

[Table("CATEGORIAS")]
public partial class Categoria
{
    [Key]
    [Column("CategoriaID")]
    public int CategoriaId { get; set; }

    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [Column("CategoriaPadreID")]
    public int? CategoriaPadreId { get; set; }

    [ForeignKey("CategoriaPadreId")]
    [InverseProperty("InverseCategoriaPadre")]
    public virtual Categoria? CategoriaPadre { get; set; }

    [InverseProperty("CategoriaPadre")]
    public virtual ICollection<Categoria> InverseCategoriaPadre { get; set; } = new List<Categoria>();

    [InverseProperty("Categoria")]
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
