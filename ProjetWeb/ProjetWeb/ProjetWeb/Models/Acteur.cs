using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjetWeb.Models;

public partial class Acteur
{
    [Key]
    public int NoActeur { get; set; }

    [StringLength(50)]
    public string Nom { get; set; } = null!;

    [StringLength(1)]
    public string? Sexe { get; set; }

    [ForeignKey("NoActeur")]
    [InverseProperty("NoActeurs")]
    public virtual ICollection<Film> NoFilms { get; set; } = new List<Film>();
}
