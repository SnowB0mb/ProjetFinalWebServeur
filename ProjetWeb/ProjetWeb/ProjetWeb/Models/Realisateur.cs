using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjetWeb.Models;

public partial class Realisateur
{
    [Key]
    public int NoRealisateur { get; set; }

    [StringLength(50)]
    public string Nom { get; set; } = null!;

    [InverseProperty("NoRealisateurNavigation")]
    public virtual ICollection<Film> Films { get; set; } = new List<Film>();
}
