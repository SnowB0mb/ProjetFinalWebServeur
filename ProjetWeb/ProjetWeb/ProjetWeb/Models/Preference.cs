using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjetWeb.Models;

public partial class Preference
{
    [Key]
    public int NoPreference { get; set; }

    [StringLength(50)]
    public string Description { get; set; } = null!;

    [InverseProperty("NoPreferenceNavigation")]
    public virtual ICollection<ValeursPreference> ValeursPreferences { get; set; } = new List<ValeursPreference>();

    [ForeignKey("NoPreference")]
    [InverseProperty("NoPreferences")]
    public virtual ICollection<Utilisateur> NoUtilisateurs { get; set; } = new List<Utilisateur>();
}
