﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VitrineApi.Models;

public partial class CategoriaProduto
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    [Unicode(false)]
    public string Titulo { get; set; }

    [Required]
    [StringLength(255)]
    [Unicode(false)]
    public string Imagem { get; set; }

    [InverseProperty("IdCategoriaProdutoNavigation")]
    public virtual ICollection<Produto> Produto { get; set; } = new List<Produto>();
}