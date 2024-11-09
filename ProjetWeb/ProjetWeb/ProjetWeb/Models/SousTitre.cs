using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjetWeb.Models;

public partial class SousTitre
{
    [Key]
    public int NoSousTitre { get; set; }

    [StringLength(10)]
    public string LangueSousTitre { get; set; } = null!;

    [ForeignKey("NoSousTitre")]
    [InverseProperty("NoSousTitres")]
    public virtual ICollection<Film> NoFilms { get; set; } = new List<Film>();
}
