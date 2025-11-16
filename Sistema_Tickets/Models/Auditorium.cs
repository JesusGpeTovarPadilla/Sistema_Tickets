using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sistema_Tickets.Models;

[Table("AUDITORIA")]
[Index("Fecha", Name = "IX_AUDITORIA_Fecha")]
public partial class Auditorium
{
    [Key]
    [Column("AuditoriaID")]
    public int AuditoriaId { get; set; }

    [Column("UsuarioID")]
    public int UsuarioId { get; set; }

    [StringLength(100)]
    public string Entidad { get; set; } = null!;

    [StringLength(100)]
    public string EntidadClave { get; set; } = null!;

    [StringLength(50)]
    public string Accion { get; set; } = null!;

    public string? DatosAntes { get; set; }

    public string? DatosDespues { get; set; }

    [Column("IP")]
    [StringLength(50)]
    public string? Ip { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Fecha { get; set; }

    [ForeignKey("UsuarioId")]
    [InverseProperty("Auditoria")]
    public virtual Usuario Usuario { get; set; } = null!;
}
