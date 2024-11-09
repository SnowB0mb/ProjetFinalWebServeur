using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjetWeb.Models;

public partial class EmpruntsFilm
{
    [Key]
    public int NoExemplaire { get; set; }

    public int NoUtilisateur { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime DateEmprunt { get; set; }

    [ForeignKey("NoExemplaire")]
    [InverseProperty("EmpruntsFilm")]
    public virtual Exemplaire NoExemplaireNavigation { get; set; } = null!;

    [ForeignKey("NoUtilisateur")]
    [InverseProperty("EmpruntsFilms")]
    public virtual Utilisateur NoUtilisateurNavigation { get; set; } = null!;
}
