using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjetWeb.Models;

public partial class Film
{
    [Key]
    public int NoFilm { get; set; }

    public int? AnneeSortie { get; set; }

    public int? Categorie { get; set; }

    public int? Format { get; set; }

    [Column("DateMAJ", TypeName = "datetime")]
    public DateTime DateMaj { get; set; }

    [Column("NoUtilisateurMAJ")]
    public int NoUtilisateurMaj { get; set; }

    public string? Resume { get; set; }

    public int? DureeMinutes { get; set; }

    public bool? FilmOriginal { get; set; }

    [StringLength(50)]
    public string? ImagePochette { get; set; }

    public int? NbDisques { get; set; }

    [StringLength(50)]
    public string? TitreFrancais { get; set; }

    [StringLength(50)]
    public string? TitreOriginal { get; set; }

    public bool? VersionEtendue { get; set; }

    public int? NoRealisateur { get; set; }

    public int? NoProducteur { get; set; }

    [StringLength(255)]
    public string? Xtra { get; set; }

    [ForeignKey("Categorie")]
    [InverseProperty("Films")]
    public virtual Category? CategorieNavigation { get; set; }

    [ForeignKey("Format")]
    [InverseProperty("Films")]
    public virtual Format? FormatNavigation { get; set; }

    [ForeignKey("NoProducteur")]
    [InverseProperty("Films")]
    public virtual Producteur? NoProducteurNavigation { get; set; }

    [ForeignKey("NoRealisateur")]
    [InverseProperty("Films")]
    public virtual Realisateur? NoRealisateurNavigation { get; set; }

    [ForeignKey("NoUtilisateurMaj")]
    [InverseProperty("Films")]
    public virtual Utilisateur NoUtilisateurMajNavigation { get; set; } = null!;

    [ForeignKey("NoFilm")]
    [InverseProperty("NoFilms")]
    public virtual ICollection<Acteur> NoActeurs { get; set; } = new List<Acteur>();

    [ForeignKey("NoFilm")]
    [InverseProperty("NoFilms")]
    public virtual ICollection<Langue> NoLangues { get; set; } = new List<Langue>();

    [ForeignKey("NoFilm")]
    [InverseProperty("NoFilms")]
    public virtual ICollection<SousTitre> NoSousTitres { get; set; } = new List<SousTitre>();

    [ForeignKey("NoFilm")]
    [InverseProperty("NoFilms")]
    public virtual ICollection<Supplement> NoSupplements { get; set; } = new List<Supplement>();
}
