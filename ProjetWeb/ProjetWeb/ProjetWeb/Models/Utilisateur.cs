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

    [StringLength(10, ErrorMessage = "Le nom d'utilisateur doit faire 10 caractères ou moins")]
    [Required(ErrorMessage = "Le nom ne doit pas être vide")]
    public string NomUtilisateur { get; set; } = null!;

    [StringLength(50, ErrorMessage = "Le courriel doit faire 50 caractères ou moins")]
    [Required(ErrorMessage = "Le courriel ne doit pas être vide")]
    public string Courriel { get; set; } = null!;

    [Required(ErrorMessage = "Le mot de passe ne doit pas être vide")]
    [Range(11111, 99999, ErrorMessage = "Le mot de passe doit être un nombre entre 11111 et 99999")]
    public int MotPasse { get; set; }

    [NotMapped]
    public int? ConfirmMotPasse { get; set; }

    [StringLength(1)]
    [Required(ErrorMessage = "Le type d'utilisateur est requis")]
    public string TypeUtilisateur { get; set; } = null!;

    [InverseProperty("NoUtilisateurNavigation")]
    public virtual ICollection<EmpruntsFilm> EmpruntsFilms { get; set; } = new List<EmpruntsFilm>();

    [InverseProperty("NoUtilisateurProprietaireNavigation")]
    public virtual ICollection<Exemplaire> Exemplaires { get; set; } = new List<Exemplaire>();

    [InverseProperty("NoUtilisateurMajNavigation")]
    public virtual ICollection<Film> Films { get; set; } = new List<Film>();

    [ForeignKey("TypeUtilisateur")]
    [InverseProperty("Utilisateurs")]
    public virtual TypesUtilisateur? TypeUtilisateurNavigation { get; set; }

    [InverseProperty("NoUtilisateurNavigation")]
    public virtual ICollection<ValeursPreference> ValeursPreferences { get; set; } = new List<ValeursPreference>();

    [ForeignKey("NoUtilisateur")]
    [InverseProperty("NoUtilisateurs")]
    public virtual ICollection<Preference> NoPreferences { get; set; } = new List<Preference>();
}
