using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjetWeb.Models;

public partial class Format
{
    [Key]
    public int NoFormat { get; set; }

    [StringLength(50)]
    public string Description { get; set; } = null!;

    [InverseProperty("FormatNavigation")]
    public virtual ICollection<Film> Films { get; set; } = new List<Film>();
}
