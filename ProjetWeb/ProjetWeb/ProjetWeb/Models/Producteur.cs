using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjetWeb.Models;

public partial class Producteur
{
    [Key]
    public int NoProducteur { get; set; }

    [StringLength(50)]
    public string Nom { get; set; } = null!;

    [InverseProperty("NoProducteurNavigation")]
    public virtual ICollection<Film> Films { get; set; } = new List<Film>();
}
