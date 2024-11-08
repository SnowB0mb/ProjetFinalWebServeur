using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjetWeb.Models;

public partial class Utilisateur
{
    [Key]
    public int NoUtilisateur { get; set; }

    [StringLength(10)]
    public string NomUtilisateur { get; set; } = null!;

    [StringLength(50)]
    public string Courriel { get; set; } = null!;

    public int MotPasse { get; set; }

    [StringLength(1)]
    public string TypeUtilisateur { get; set; } = null!;

    [InverseProperty("NoUtilisateurNavigation")]
    public virtual ICollection<EmpruntsFilm> EmpruntsFilms { get; set; } = new List<EmpruntsFilm>();

    [InverseProperty("NoUtilisateurProprietaireNavigation")]
    public virtual ICollection<Exemplaire> Exemplaires { get; set; } = new List<Exemplaire>();

    [InverseProperty("NoUtilisateurMajNavigation")]
    public virtual ICollection<Film> Films { get; set; } = new List<Film>();

    [ForeignKey("TypeUtilisateur")]
    [InverseProperty("Utilisateurs")]
    public virtual TypesUtilisateur TypeUtilisateurNavigation { get; set; } = null!;

    [InverseProperty("NoUtilisateurNavigation")]
    public virtual ICollection<ValeursPreference> ValeursPreferences { get; set; } = new List<ValeursPreference>();

    [ForeignKey("NoUtilisateur")]
    [InverseProperty("NoUtilisateurs")]
    public virtual ICollection<Preference> NoPreferences { get; set; } = new List<Preference>();
}
