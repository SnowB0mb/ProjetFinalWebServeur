using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ProjetWeb.Models;

public partial class Bdw56a24E1Context : DbContext
{
    public Bdw56a24E1Context()
    {
    }

    public Bdw56a24E1Context(DbContextOptions<Bdw56a24E1Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Acteur> Acteurs { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<EmpruntsFilm> EmpruntsFilms { get; set; }

    public virtual DbSet<Exemplaire> Exemplaires { get; set; }

    public virtual DbSet<Film> Films { get; set; }

    public virtual DbSet<Format> Formats { get; set; }

    public virtual DbSet<Langue> Langues { get; set; }

    public virtual DbSet<Preference> Preferences { get; set; }

    public virtual DbSet<Producteur> Producteurs { get; set; }

    public virtual DbSet<Realisateur> Realisateurs { get; set; }

    public virtual DbSet<SousTitre> SousTitres { get; set; }

    public virtual DbSet<Supplement> Supplements { get; set; }

    public virtual DbSet<TypesUtilisateur> TypesUtilisateurs { get; set; }

    public virtual DbSet<Utilisateur> Utilisateurs { get; set; }

    public virtual DbSet<ValeursPreference> ValeursPreferences { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=tcp:424sql.cgodin.qc.ca,5433;Database=BDW56A24_E1;User Id=W56A24equipe1;Password=Secret43862;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Acteur>(entity =>
        {
            entity.HasKey(e => e.NoActeur).HasName("PK__Acteurs__CB047685727D80B6");

            entity.Property(e => e.NoActeur).ValueGeneratedNever();
            entity.Property(e => e.Nom).HasMaxLength(50);
            entity.Property(e => e.Sexe)
                .HasMaxLength(1)
                .IsFixedLength();
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.NoCategorie).HasName("PK__Categori__8F1253B90AF2DB8A");

            entity.Property(e => e.NoCategorie).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(50);
        });

        modelBuilder.Entity<EmpruntsFilm>(entity =>
        {
            entity.HasKey(e => e.NoExemplaire).HasName("PK__Emprunts__9D25C47A5AA61FB6");

            entity.Property(e => e.NoExemplaire).ValueGeneratedNever();
            entity.Property(e => e.DateEmprunt).HasColumnType("datetime");

            entity.HasOne(d => d.NoExemplaireNavigation).WithOne(p => p.EmpruntsFilm)
                .HasForeignKey<EmpruntsFilm>(d => d.NoExemplaire)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EmpruntsF__NoExe__68487DD7");

            entity.HasOne(d => d.NoUtilisateurNavigation).WithMany(p => p.EmpruntsFilms)
                .HasForeignKey(d => d.NoUtilisateur)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EmpruntsF__NoUti__693CA210");
        });

        modelBuilder.Entity<Exemplaire>(entity =>
        {
            entity.HasKey(e => e.NoExemplaire).HasName("PK__Exemplai__9D25C47ACB51AC3B");

            entity.Property(e => e.NoExemplaire).ValueGeneratedNever();

            entity.HasOne(d => d.NoUtilisateurProprietaireNavigation).WithMany(p => p.Exemplaires)
                .HasForeignKey(d => d.NoUtilisateurProprietaire)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Exemplair__NoUti__6A30C649");
        });

        modelBuilder.Entity<Film>(entity =>
        {
            entity.HasKey(e => e.NoFilm).HasName("PK__Films__0925D31B246835D5");

            entity.Property(e => e.NoFilm).ValueGeneratedNever();
            entity.Property(e => e.DateMaj)
                .HasColumnType("datetime")
                .HasColumnName("DateMAJ");
            entity.Property(e => e.ImagePochette).HasMaxLength(50);
            entity.Property(e => e.NoUtilisateurMaj).HasColumnName("NoUtilisateurMAJ");
            entity.Property(e => e.TitreFrancais).HasMaxLength(50);
            entity.Property(e => e.TitreOriginal).HasMaxLength(50);
            entity.Property(e => e.Xtra).HasMaxLength(255);

            entity.HasOne(d => d.CategorieNavigation).WithMany(p => p.Films)
                .HasForeignKey(d => d.Categorie)
                .HasConstraintName("FK__Films__Categorie__6B24EA82");

            entity.HasOne(d => d.FormatNavigation).WithMany(p => p.Films)
                .HasForeignKey(d => d.Format)
                .HasConstraintName("FK__Films__Format__6C190EBB");

            entity.HasOne(d => d.NoProducteurNavigation).WithMany(p => p.Films)
                .HasForeignKey(d => d.NoProducteur)
                .HasConstraintName("FK__Films__NoProduct__6EF57B66");

            entity.HasOne(d => d.NoRealisateurNavigation).WithMany(p => p.Films)
                .HasForeignKey(d => d.NoRealisateur)
                .HasConstraintName("FK__Films__NoRealisa__6E01572D");

            entity.HasOne(d => d.NoUtilisateurMajNavigation).WithMany(p => p.Films)
                .HasForeignKey(d => d.NoUtilisateurMaj)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Films__NoUtilisa__6D0D32F4");

            entity.HasMany(d => d.NoActeurs).WithMany(p => p.NoFilms)
                .UsingEntity<Dictionary<string, object>>(
                    "FilmsActeur",
                    r => r.HasOne<Acteur>().WithMany()
                        .HasForeignKey("NoActeur")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__FilmsActe__NoAct__70DDC3D8"),
                    l => l.HasOne<Film>().WithMany()
                        .HasForeignKey("NoFilm")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__FilmsActe__NoFil__6FE99F9F"),
                    j =>
                    {
                        j.HasKey("NoFilm", "NoActeur").HasName("PK__FilmsAct__45959473FF9ABDC9");
                        j.ToTable("FilmsActeurs");
                    });

            entity.HasMany(d => d.NoLangues).WithMany(p => p.NoFilms)
                .UsingEntity<Dictionary<string, object>>(
                    "FilmsLangue",
                    r => r.HasOne<Langue>().WithMany()
                        .HasForeignKey("NoLangue")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__FilmsLang__NoLan__72C60C4A"),
                    l => l.HasOne<Film>().WithMany()
                        .HasForeignKey("NoFilm")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__FilmsLang__NoFil__71D1E811"),
                    j =>
                    {
                        j.HasKey("NoFilm", "NoLangue").HasName("PK__FilmsLan__E9AF069878B249A2");
                        j.ToTable("FilmsLangues");
                    });

            entity.HasMany(d => d.NoSousTitres).WithMany(p => p.NoFilms)
                .UsingEntity<Dictionary<string, object>>(
                    "FilmsSousTitre",
                    r => r.HasOne<SousTitre>().WithMany()
                        .HasForeignKey("NoSousTitre")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__FilmsSous__NoSou__74AE54BC"),
                    l => l.HasOne<Film>().WithMany()
                        .HasForeignKey("NoFilm")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__FilmsSous__NoFil__73BA3083"),
                    j =>
                    {
                        j.HasKey("NoFilm", "NoSousTitre").HasName("PK__FilmsSou__12FA8392526C8604");
                        j.ToTable("FilmsSousTitres");
                    });

            entity.HasMany(d => d.NoSupplements).WithMany(p => p.NoFilms)
                .UsingEntity<Dictionary<string, object>>(
                    "FilmsSupplement",
                    r => r.HasOne<Supplement>().WithMany()
                        .HasForeignKey("NoSupplement")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__FilmsSupp__NoSup__76969D2E"),
                    l => l.HasOne<Film>().WithMany()
                        .HasForeignKey("NoFilm")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__FilmsSupp__NoFil__75A278F5"),
                    j =>
                    {
                        j.HasKey("NoFilm", "NoSupplement").HasName("PK__FilmsSup__AA85C214771AF548");
                        j.ToTable("FilmsSupplements");
                    });
        });

        modelBuilder.Entity<Format>(entity =>
        {
            entity.HasKey(e => e.NoFormat).HasName("PK__Formats__14C9A89D754921C7");

            entity.Property(e => e.NoFormat).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(50);
        });

        modelBuilder.Entity<Langue>(entity =>
        {
            entity.HasKey(e => e.NoLangue).HasName("PK__Langues__08AD583F40C35DD2");

            entity.Property(e => e.NoLangue).ValueGeneratedNever();
            entity.Property(e => e.Langue1)
                .HasMaxLength(10)
                .HasColumnName("Langue");
        });

        modelBuilder.Entity<Preference>(entity =>
        {
            entity.HasKey(e => e.NoPreference).HasName("PK__Preferen__625F5DC993EEE7C1");

            entity.Property(e => e.NoPreference).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(50);
        });

        modelBuilder.Entity<Producteur>(entity =>
        {
            entity.HasKey(e => e.NoProducteur).HasName("PK__Producte__65CE0B72E88C25AE");

            entity.Property(e => e.NoProducteur).ValueGeneratedNever();
            entity.Property(e => e.Nom).HasMaxLength(50);
        });

        modelBuilder.Entity<Realisateur>(entity =>
        {
            entity.HasKey(e => e.NoRealisateur).HasName("PK__Realisat__10EE19F6258BC980");

            entity.Property(e => e.NoRealisateur).ValueGeneratedNever();
            entity.Property(e => e.Nom).HasMaxLength(50);
        });

        modelBuilder.Entity<SousTitre>(entity =>
        {
            entity.HasKey(e => e.NoSousTitre).HasName("PK__SousTitr__BDF508903F764D95");

            entity.Property(e => e.NoSousTitre).ValueGeneratedNever();
            entity.Property(e => e.LangueSousTitre).HasMaxLength(10);
        });

        modelBuilder.Entity<Supplement>(entity =>
        {
            entity.HasKey(e => e.NoSupplement).HasName("PK__Suppleme__3A0110FB18304C14");

            entity.Property(e => e.NoSupplement).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(50);
        });

        modelBuilder.Entity<TypesUtilisateur>(entity =>
        {
            entity.HasKey(e => e.TypeUtilisateur).HasName("PK__TypesUti__4B039DB650B12458");

            entity.ToTable("TypesUtilisateur");

            entity.Property(e => e.TypeUtilisateur)
                .HasMaxLength(1)
                .IsFixedLength();
            entity.Property(e => e.Description).HasMaxLength(25);
        });

        modelBuilder.Entity<Utilisateur>(entity =>
        {
            entity.HasKey(e => e.NoUtilisateur).HasName("PK__Utilisat__131985C44F51B126");

            entity.Property(e => e.NoUtilisateur).ValueGeneratedNever();
            entity.Property(e => e.Courriel).HasMaxLength(50);
            entity.Property(e => e.NomUtilisateur).HasMaxLength(10);
            entity.Property(e => e.TypeUtilisateur)
                .HasMaxLength(1)
                .IsFixedLength();

            entity.HasOne(d => d.TypeUtilisateurNavigation).WithMany(p => p.Utilisateurs)
                .HasForeignKey(d => d.TypeUtilisateur)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Utilisate__TypeU__778AC167");

            entity.HasMany(d => d.NoPreferences).WithMany(p => p.NoUtilisateurs)
                .UsingEntity<Dictionary<string, object>>(
                    "UtilisateursPreference",
                    r => r.HasOne<Preference>().WithMany()
                        .HasForeignKey("NoPreference")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Utilisate__NoPre__797309D9"),
                    l => l.HasOne<Utilisateur>().WithMany()
                        .HasForeignKey("NoUtilisateur")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Utilisate__NoUti__787EE5A0"),
                    j =>
                    {
                        j.HasKey("NoUtilisateur", "NoPreference").HasName("PK__Utilisat__953C7018164233D3");
                        j.ToTable("UtilisateursPreferences");
                    });
        });

        modelBuilder.Entity<ValeursPreference>(entity =>
        {
            entity.HasKey(e => new { e.NoUtilisateur, e.NoPreference }).HasName("PK__ValeursP__953C7018A155CE2E");

            entity.Property(e => e.Valeur).HasMaxLength(50);

            entity.HasOne(d => d.NoPreferenceNavigation).WithMany(p => p.ValeursPreferences)
                .HasForeignKey(d => d.NoPreference)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ValeursPr__NoPre__7B5B524B");

            entity.HasOne(d => d.NoUtilisateurNavigation).WithMany(p => p.ValeursPreferences)
                .HasForeignKey(d => d.NoUtilisateur)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ValeursPr__NoUti__7A672E12");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
