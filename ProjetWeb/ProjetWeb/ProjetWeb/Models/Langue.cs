using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjetWeb.Models;

public partial class Langue
{
    [Key]
    public int NoLangue { get; set; }

    [Column("Langue")]
    [StringLength(10)]
    public string Langue1 { get; set; } = null!;

    [ForeignKey("NoLangue")]
    [InverseProperty("NoLangues")]
    public virtual ICollection<Film> NoFilms { get; set; } = new List<Film>();
}
