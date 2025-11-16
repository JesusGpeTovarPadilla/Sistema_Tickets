using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sistema_Tickets.Models;

[Table("CONFIGURACIONEMPRESA")]
public partial class Configuracionempresa
{
    [Key]
    [Column("ConfigID")]
    public int ConfigId { get; set; }

    [StringLength(200)]
    public string NombreEmpresa { get; set; } = null!;

    [StringLength(200)]
    public string? RazonSocial { get; set; }

    [Column("RFC")]
    [StringLength(20)]
    public string? Rfc { get; set; }

    [StringLength(150)]
    public string? Calle { get; set; }

    [StringLength(20)]
    public string? NumeroExterior { get; set; }

    [StringLength(20)]
    public string? NumeroInterior { get; set; }

    [StringLength(100)]
    public string? Colonia { get; set; }

    [StringLength(100)]
    public string? Ciudad { get; set; }

    [StringLength(100)]
    public string? Estado { get; set; }

    [StringLength(10)]
    public string? CodigoPostal { get; set; }

    [StringLength(100)]
    public string? Pais { get; set; }

    [StringLength(50)]
    public string? Telefono { get; set; }

    [StringLength(150)]
    public string? Correo { get; set; }

    [StringLength(150)]
    public string? SitioWeb { get; set; }

    public string? Logo { get; set; }

    [StringLength(10)]
    public string? Moneda { get; set; }

    [StringLength(10)]
    public string? Lenguaje { get; set; }

    [StringLength(100)]
    public string? ZonaHoraria { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaModificacion { get; set; }

    [StringLength(100)]
    public string? UsuarioModificacion { get; set; }
}
