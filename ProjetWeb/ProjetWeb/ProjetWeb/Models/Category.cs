using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjetWeb.Models;

public partial class Category
{
    [Key]
    public int NoCategorie { get; set; }

    [StringLength(50)]
    public string Description { get; set; } = null!;

    [InverseProperty("CategorieNavigation")]
    public virtual ICollection<Film> Films { get; set; } = new List<Film>();
}
