using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetWeb.Models;

namespace ProjetWeb.Controllers
{
    public class FilmsController : Controller
    {
        private readonly FilmDbContext _context;
        public const string SessionKeyId = "_Id";
        private int? _userIdConnected => HttpContext.Session.GetInt32(SessionKeyId);

        // à des fins de déboggages, changer la valeur a true
        public bool IsConnected => HttpContext.Session.GetInt32(SessionKeyId) > -1;

        public FilmsController(FilmDbContext context)
        {
            _context = context;
        }

        //private void InitializeUserId()
        //{
        //    if (_userIdConnected == null)
        //    {
        //        //_userIdConnected = HttpContext.Session.GetInt32(SessionKeyId);
        //    }
        //}

        // GET: Films
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 12, string searchString = "", string sortOrder = "")
        {
            if (!IsConnected)
            {
                return Redirect("/Home/Index");
            }

            // Calculer nb total de films + nb total de pages
            var totalFilms = await _context.Films.CountAsync();
            var totalPages = (int)Math.Ceiling(totalFilms / (double)pageSize);

            // Vérifier si page demandée valide
            if (pageNumber < 1) pageNumber = 1;
            if (pageNumber > totalPages) pageNumber = totalPages;

            // Requête de base pour récupérer les films
            IQueryable<Film> filmsQuery = _context.Films
                .Include(f => f.NoUtilisateurMajNavigation)
                .AsQueryable();

            // Trier selon searchQuery
            if (!String.IsNullOrEmpty(searchString))
            {
                filmsQuery = filmsQuery.Where(f => f.TitreFrancais.Contains(searchString) || f.TitreOriginal.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "utilisteur":
                    filmsQuery = filmsQuery.OrderBy(f => f.NoUtilisateurMajNavigation.NomUtilisateur);
                    break;
                case "titre":
                    filmsQuery = filmsQuery.OrderBy(f => f.TitreFrancais);
                    break;
                case "utilisateur_titre":
                    filmsQuery = filmsQuery.OrderBy(f => f.NoUtilisateurMajNavigation.NomUtilisateur).ThenBy(f => f.TitreFrancais);
                    break;
                default:
                    filmsQuery = filmsQuery.OrderBy(f => f.TitreFrancais);
                    break;
            }

            // Appliquer pagination
            var films = await filmsQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Passer les données de pagination à la vue
            ViewData["CurrentPage"] = pageNumber;
            ViewData["TotalPages"] = totalPages;
            ViewData["PageSize"] = pageSize;
            ViewData["SortOrder"] = sortOrder;
            ViewData["SearchString"] = searchString;
            ViewData["CurrentUser"] = _userIdConnected;

            return View(films);
        }

        // GET: Films/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsConnected)
            {
                return Redirect("/Home/Index");
            }
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Films
                .Include(f => f.CategorieNavigation)
                .Include(f => f.FormatNavigation)
                .Include(f => f.NoProducteurNavigation)
                .Include(f => f.NoRealisateurNavigation)
                .Include(f => f.NoUtilisateurMajNavigation)
                .FirstOrDefaultAsync(m => m.NoFilm == id);
            if (film == null)
            {
                return NotFound();
            }

            return View(film);
        }

        // GET: Films/Create
        public IActionResult Create()
        {
            if (!IsConnected)
            {
                return Redirect("/Home/Index");
            }
            ViewData["Categorie"] = new SelectList(_context.Categories, "NoCategorie", "Description");
            ViewData["Format"] = new SelectList(_context.Formats, "NoFormat", "Description");
            ViewData["NoProducteur"] = new SelectList(_context.Producteurs, "NoProducteur", "Nom");
            ViewData["NoRealisateur"] = new SelectList(_context.Realisateurs, "NoRealisateur", "Nom");
            ViewData["NoUtilisateurMaj"] = new SelectList(_context.Utilisateurs, "NoUtilisateur", "NomUtilisateur");
            FilmViewModel filmViewModel = new FilmViewModel();
            return View(filmViewModel);
        }

        // POST: Films/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FilmViewModel filmViewModel)
        {

            if (!IsConnected)
            {
                return Redirect("/Home/Index");
            }
            filmViewModel.Film.NoFilm = _context.Films.Max(f => f.NoFilm) + 1;
            filmViewModel.Film.DateMaj = DateTime.Now;
            filmViewModel.Film.NoUtilisateurMaj = _userIdConnected ?? 1;
            filmViewModel.Film.FilmOriginal = Convert.ToBoolean(Request.Form["checkFilmOriginal"]);
            filmViewModel.Film.VersionEtendue = Convert.ToBoolean(Request.Form["checkVersionEtendue"]);
            if (filmViewModel.Image != null)
            {
                string chemin = "wwwroot/images/";
                string nomFichier = filmViewModel.Film.NoFilm; + Path.GetExtension(filmViewModel.Image.FileName);
                filmViewModel.Film.ImagePochette = nomFichier;
                string cheminFichier = chemin + nomFichier;

                //Directory.CreateDirectory(cheminFichier);

                using (var fileStream = new FileStream(cheminFichier, FileMode.Create))
                {
                    await filmViewModel.Image.CopyToAsync(fileStream);
                }
            }
            if (ModelState.IsValid)
            {
                //Gestion importation image
                _context.Add(filmViewModel.Film);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Categorie"] = new SelectList(_context.Categories, "NoCategorie", "Description", filmViewModel.Film.Categorie);
            ViewData["Format"] = new SelectList(_context.Formats, "NoFormat", "Description", filmViewModel.Film.Format);
            ViewData["NoProducteur"] = new SelectList(_context.Producteurs, "NoProducteur", "Nom", filmViewModel.Film.NoProducteur);
            ViewData["NoRealisateur"] = new SelectList(_context.Realisateurs, "NoRealisateur", "Nom", filmViewModel.Film.NoRealisateur);
            ViewData["NoUtilisateurMaj"] = new SelectList(_context.Utilisateurs, "NoUtilisateur", "NomUtilisateur", filmViewModel.Film.NoUtilisateurMaj);
            return View(filmViewModel);
        }

        // GET: Films/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsConnected)
            {
                return Redirect("/Home/Index");
            }
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Films.FindAsync(id);
            if (film == null)
            {
                return NotFound();
            }
            ViewData["Categorie"] = new SelectList(_context.Categories, "NoCategorie", "NoCategorie", film.Categorie);
            ViewData["Format"] = new SelectList(_context.Formats, "NoFormat", "NoFormat", film.Format);
            ViewData["NoProducteur"] = new SelectList(_context.Producteurs, "NoProducteur", "NoProducteur", film.NoProducteur);
            ViewData["NoRealisateur"] = new SelectList(_context.Realisateurs, "NoRealisateur", "NoRealisateur", film.NoRealisateur);
            ViewData["NoUtilisateurMaj"] = new SelectList(_context.Utilisateurs, "NoUtilisateur", "NoUtilisateur", film.NoUtilisateurMaj);
            return View(film);
        }

        // POST: Films/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NoFilm,AnneeSortie,Categorie,Format,DateMaj,NoUtilisateurMaj,Resume,DureeMinutes,FilmOriginal,ImagePochette,NbDisques,TitreFrancais,TitreOriginal,VersionEtendue,NoRealisateur,NoProducteur,Xtra")] Film film)
        {
            if (!IsConnected)
            {
                return Redirect("/Home/Index");
            }
            if (id != film.NoFilm)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(film);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilmExists(film.NoFilm))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Categorie"] = new SelectList(_context.Categories, "NoCategorie", "NoCategorie", film.Categorie);
            ViewData["Format"] = new SelectList(_context.Formats, "NoFormat", "NoFormat", film.Format);
            ViewData["NoProducteur"] = new SelectList(_context.Producteurs, "NoProducteur", "NoProducteur", film.NoProducteur);
            ViewData["NoRealisateur"] = new SelectList(_context.Realisateurs, "NoRealisateur", "NoRealisateur", film.NoRealisateur);
            ViewData["NoUtilisateurMaj"] = new SelectList(_context.Utilisateurs, "NoUtilisateur", "NoUtilisateur", film.NoUtilisateurMaj);
            return View(film);
        }

        // GET: Films/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsConnected)
            {
                return Redirect("/Home/Index");
            }
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Films
                .Include(f => f.CategorieNavigation)
                .Include(f => f.FormatNavigation)
                .Include(f => f.NoProducteurNavigation)
                .Include(f => f.NoRealisateurNavigation)
                .Include(f => f.NoUtilisateurMajNavigation)
                .FirstOrDefaultAsync(m => m.NoFilm == id);
            if (film == null)
            {
                return NotFound();
            }

            return View(film);
        }

        // POST: Films/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsConnected)
            {
                return Redirect("/Home/Index");
            }
            var film = await _context.Films.FindAsync(id);
            if (film != null)
            {
                _context.Films.Remove(film);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FilmExists(int id)
        {
            return _context.Films.Any(e => e.NoFilm == id);
        }
    }
}
