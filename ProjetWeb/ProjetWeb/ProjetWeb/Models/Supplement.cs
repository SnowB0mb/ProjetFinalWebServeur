using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjetWeb.Models;

public partial class Supplement
{
    [Key]
    public int NoSupplement { get; set; }

    [StringLength(50)]
    public string Description { get; set; } = null!;

    [ForeignKey("NoSupplement")]
    [InverseProperty("NoSupplements")]
    public virtual ICollection<Film> NoFilms { get; set; } = new List<Film>();
}
