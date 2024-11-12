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

        // à des fins de déboggages, changer la valeur a true
        public bool IsConnected => HttpContext.Session.GetInt32(SessionKeyId) > -1;

        public FilmsController(FilmDbContext context)
        {
            _context = context;
        }

        // GET: Films
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 12)
        {
            if (!IsConnected)
            {
                return Redirect("/Home/Index");
            }

            // Calculer le nombre total de films
            var totalFilms = await _context.Films.CountAsync();

            // Calculer le nombre total de pages
            var totalPages = (int)Math.Ceiling(totalFilms / (double)pageSize);

            // Vérifier si la page demandée est valide
            if (pageNumber < 1) pageNumber = 1;
            if (pageNumber > totalPages) pageNumber = totalPages;

            // Obtenir la liste des films pour la page demandée
            var films = await _context.Films
                .Include(f => f.NoUtilisateurMajNavigation)
                .OrderBy(f => f.TitreFrancais) // Trier par titre
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Passer les données de pagination à la vue
            ViewData["CurrentPage"] = pageNumber;
            ViewData["TotalPages"] = totalPages;
            ViewData["PageSize"] = pageSize;

            return View(films);
            //var filmDbContext = _context.Films.Include(f => f.CategorieNavigation).Include(f => f.FormatNavigation).Include(f => f.NoProducteurNavigation).Include(f => f.NoRealisateurNavigation).Include(f => f.NoUtilisateurMajNavigation);
            //return View(await filmDbContext.ToListAsync());
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
            ViewData["Categorie"] = new SelectList(_context.Categories, "NoCategorie", "NoCategorie");
            ViewData["Format"] = new SelectList(_context.Formats, "NoFormat", "NoFormat");
            ViewData["NoProducteur"] = new SelectList(_context.Producteurs, "NoProducteur", "NoProducteur");
            ViewData["NoRealisateur"] = new SelectList(_context.Realisateurs, "NoRealisateur", "NoRealisateur");
            ViewData["NoUtilisateurMaj"] = new SelectList(_context.Utilisateurs, "NoUtilisateur", "NoUtilisateur");
            return View();
        }

        // POST: Films/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NoFilm,AnneeSortie,Categorie,Format,DateMaj,NoUtilisateurMaj,Resume,DureeMinutes,FilmOriginal,ImagePochette,NbDisques,TitreFrancais,TitreOriginal,VersionEtendue,NoRealisateur,NoProducteur,Xtra")] Film film)
        {
            if (!IsConnected)
            {
                return Redirect("/Home/Index");
            }
            if (ModelState.IsValid)
            {
                _context.Add(film);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Categorie"] = new SelectList(_context.Categories, "NoCategorie", "NoCategorie", film.Categorie);
            ViewData["Format"] = new SelectList(_context.Formats, "NoFormat", "NoFormat", film.Format);
            ViewData["NoProducteur"] = new SelectList(_context.Producteurs, "NoProducteur", "NoProducteur", film.NoProducteur);
            ViewData["NoRealisateur"] = new SelectList(_context.Realisateurs, "NoRealisateur", "NoRealisateur", film.NoRealisateur);
            ViewData["NoUtilisateurMaj"] = new SelectList(_context.Utilisateurs, "NoUtilisateur", "NoUtilisateur", film.NoUtilisateurMaj);
            return View(film);
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
