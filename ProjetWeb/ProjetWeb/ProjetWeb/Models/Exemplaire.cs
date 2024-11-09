using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjetWeb.Models;

public partial class Exemplaire
{
    [Key]
    public int NoExemplaire { get; set; }

    public int NoUtilisateurProprietaire { get; set; }

    [InverseProperty("NoExemplaireNavigation")]
    public virtual EmpruntsFilm? EmpruntsFilm { get; set; }

    [ForeignKey("NoUtilisateurProprietaire")]
    [InverseProperty("Exemplaires")]
    public virtual Utilisateur NoUtilisateurProprietaireNavigation { get; set; } = null!;
}
