using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjetWeb.Models;

[Table("TypesUtilisateur")]
public partial class TypesUtilisateur
{
    [Key]
    [StringLength(1)]
    public string TypeUtilisateur { get; set; } = null!;

    [StringLength(25)]
    public string Description { get; set; } = null!;

    [InverseProperty("TypeUtilisateurNavigation")]
    public virtual ICollection<Utilisateur> Utilisateurs { get; set; } = new List<Utilisateur>();
}
