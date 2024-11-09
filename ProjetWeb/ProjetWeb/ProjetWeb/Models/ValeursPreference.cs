using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjetWeb.Models;

[PrimaryKey("NoUtilisateur", "NoPreference")]
public partial class ValeursPreference
{
    [Key]
    public int NoUtilisateur { get; set; }

    [Key]
    public int NoPreference { get; set; }

    [StringLength(50)]
    public string Valeur { get; set; } = null!;

    [ForeignKey("NoPreference")]
    [InverseProperty("ValeursPreferences")]
    public virtual Preference NoPreferenceNavigation { get; set; } = null!;

    [ForeignKey("NoUtilisateur")]
    [InverseProperty("ValeursPreferences")]
    public virtual Utilisateur NoUtilisateurNavigation { get; set; } = null!;
}
