using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProjetWeb.Models;

namespace ProjetWeb.Controllers
{
    public class FilmsController : Controller
    {
        private int? _userIdConnected => HttpContext.Session.GetInt32(SessionKeyId);

        // à des fins de déboggages, changer la valeur a true
        public bool IsConnected => HttpContext.Session.GetInt32(SessionKeyId) > -1;

        private readonly FilmDbContext _context;
        public const string SessionKeyId = "_Id";
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FilmsController(FilmDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("/Email")]
        public IActionResult Email(string emailRaison, string filmTitle)
        {
            var users = new List<Utilisateur>();
            if (!IsConnected)
            {
                return Redirect("/Home/Index");
            }
            int valeurPreference = 0;

            if (emailRaison == "Les utilisateurs suivants seront notifiés de l'ajout du film:")
            {
                valeurPreference = 3;
            }
            else if (emailRaison == "Les utilisateurs suivants seront notifiés de la suppression du film : ")
            {
                valeurPreference = 5;
            }
            else if (emailRaison == "Les utilisateurs suivants seront notifiés de l'appropriation du film:")
            {
                valeurPreference = 4;
            }

            users = _context.ValeursPreferences
        .Where(vp => vp.NoPreference == valeurPreference && vp.Valeur == "1")
        .Select(vp => vp.NoUtilisateurNavigation)
        .ToList();
            var viewModel = new EmailViewModel
            {
                Users = users,
                FilmTitle = filmTitle,
                EmailRaison = emailRaison
            };
            return View(viewModel);
        }



        // methode qui retroune le type d'utilisateur
        private async Task<string> GetUserTypeAsync()
        {
            if (_userIdConnected == null)
            {
                return "Guest"; // Par défaut, retournez un type d'utilisateur invité
            }

            var utilisateur = await _context.Utilisateurs
                .Where(u => u.NoUtilisateur == _userIdConnected)
                .Select(u => u.TypeUtilisateur)
                .FirstOrDefaultAsync();

            return utilisateur ?? "Guest"; // retourne Guest si on trouve aucun utilisateur
        }

        // GET: Films
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 12, string searchString = "", string sortOrder = "", int filtrerUser = -1)
        {
            if (!IsConnected)
            {
                return Redirect("/Home/Index");
            }

            // Ajouter le type d'utilisateur dans ViewData
            ViewData["UserType"] = await GetUserTypeAsync();

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

            if (filtrerUser != -1)
            {
                filmsQuery = filmsQuery.Where(f => f.NoUtilisateurMaj == filtrerUser);
                ViewData["FiltrerUser"] = filtrerUser;
            }

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

            ViewData["BackgroundImagePath"] = HttpContext.Session.GetString("BackgroundImagePath");

            return View(films);
        }


        [HttpGet("/environement")]
        public IActionResult Environement()
        {
            if (!IsConnected)
            {
                return Redirect("/Home/Index");
            }

            int? userId = HttpContext.Session.GetInt32(SessionKeyId);
            if (userId == null)
            {
                return Redirect("/Home/Index");
            }

            var preferenceIds = new List<int> { 3, 4, 5 };
            var userPreferences = _context.ValeursPreferences
                .Where(vp => vp.NoUtilisateur == userId && preferenceIds.Contains(vp.NoPreference))
                .ToList();

            var preferences = preferenceIds.ToDictionary(
                id => id,
                id => userPreferences.FirstOrDefault(vp => vp.NoPreference == id)?.Valeur ?? "1"
            );

            ViewBag.Preferences = preferences;
            ViewData["BackgroundImagePath"] = HttpContext.Session.GetString("BackgroundImagePath");

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> UploadBackground(IFormFile backgroundImage, Dictionary<int, string> Preferences, string newPassword)
        {
            if (backgroundImage != null && backgroundImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);
                var filePath = Path.Combine(uploadsFolder, backgroundImage.FileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await backgroundImage.CopyToAsync(fileStream);
                }

                HttpContext.Session.SetString("BackgroundImagePath", $"/uploads/{backgroundImage.FileName}");
            }

            int? userId = HttpContext.Session.GetInt32(SessionKeyId);
            if (userId == null)
            {
                return RedirectToAction("Index");
            }

            var preferences = new List<ValeursPreference>
            {
                new ValeursPreference { NoUtilisateur = userId.Value, NoPreference = 3, Valeur = Preferences.ContainsKey(3) ? Preferences[3] : "0" },
                new ValeursPreference { NoUtilisateur = userId.Value, NoPreference = 4, Valeur = Preferences.ContainsKey(4) ? Preferences[4] : "0" },
                new ValeursPreference { NoUtilisateur = userId.Value, NoPreference = 5, Valeur = Preferences.ContainsKey(5) ? Preferences[5] : "0" }
            };

            foreach (var preference in preferences)
            {
                var existingPreference = _context.ValeursPreferences
                    .FirstOrDefault(vp => vp.NoUtilisateur == userId && vp.NoPreference == preference.NoPreference);
                    existingPreference.Valeur = preference.Valeur;
                    _context.Update(existingPreference);
            }

            if (!string.IsNullOrEmpty(newPassword) && int.TryParse(newPassword, out int newPasswordValue) && newPasswordValue >= 11111 && newPasswordValue <= 99999)
            {
                var user = await _context.Utilisateurs.FindAsync(userId.Value);
                if (user != null)
                {
                    user.MotPasse = newPasswordValue;
                    _context.Update(user);
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
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
            ViewData["BackgroundImagePath"] = HttpContext.Session.GetString("BackgroundImagePath");
            ViewData["CurrentUser"] = _userIdConnected;

            return View(film);
        }

        // GET: Films/Create
        public async Task<IActionResult> CreateAsync()
        {
            if (!IsConnected)
            {
                return Redirect("/Home/Index");
            }
            var userType = await GetUserTypeAsync();
            ViewData["UserType"] = userType;

            ViewData["Categorie"] = new SelectList(_context.Categories, "NoCategorie", "Description");
            ViewData["Format"] = new SelectList(_context.Formats, "NoFormat", "Description");
            ViewData["NoProducteur"] = new SelectList(_context.Producteurs, "NoProducteur", "Nom");
            ViewData["NoRealisateur"] = new SelectList(_context.Realisateurs, "NoRealisateur", "Nom");
            ViewData["NoUtilisateurMaj"] = new SelectList(_context.Utilisateurs, "NoUtilisateur", "NomUtilisateur");
            ViewData["CurrentUser"] = _userIdConnected;

            FilmViewModel filmViewModel = new FilmViewModel();
            ViewData["BackgroundImagePath"] = HttpContext.Session.GetString("BackgroundImagePath");
            return View(filmViewModel);
        }

        // POST: Films/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FilmViewModel filmViewModel, int selectedUserId)
        {

            if (!IsConnected)
            {
                return Redirect("/Home/Index");
            }
            filmViewModel.Film.NoFilm = _context.Films.Max(f => f.NoFilm) + 1;
            filmViewModel.Film.DateMaj = DateTime.Now;

            var userType = await GetUserTypeAsync();

            if (userType == "S" && selectedUserId > 0)
            {
                // Mettre à jour avec l'utilisateur sélectionné
                filmViewModel.Film.NoUtilisateurMaj = selectedUserId;
            }
            else
            {
                // Sinon, assigner l'utilisateur connecté
                filmViewModel.Film.NoUtilisateurMaj = _userIdConnected ?? 1;
            }

            filmViewModel.Film.FilmOriginal = Convert.ToBoolean(Request.Form["checkFilmOriginal"]);
            filmViewModel.Film.VersionEtendue = Convert.ToBoolean(Request.Form["checkVersionEtendue"]);
            if (filmViewModel.Image != null)
            {
                string chemin = "wwwroot/images/";
                string nomFichier = filmViewModel.Film.NoFilm + Path.GetExtension(filmViewModel.Image.FileName);
                filmViewModel.Film.ImagePochette = nomFichier;
                string cheminFichier = chemin + nomFichier;

                //Directory.CreateDirectory(cheminFichier);

                using (var fileStream = new FileStream(cheminFichier, FileMode.Create))
                {
                    await filmViewModel.Image.CopyToAsync(fileStream);
                }
            }
            ModelState.Remove("ImagePochette");
            ModelState.Remove("Image");
            if (ModelState.IsValid)
            {
                _context.Add(filmViewModel.Film);
                await _context.SaveChangesAsync();

                // Fetch users with preference 3 set to 1
                var usersToNotify = _context.ValeursPreferences
                    .Where(vp => vp.NoPreference == 3 && vp.Valeur == "1")
                    .Select(vp => vp.NoUtilisateurNavigation)
                    .ToList();

                var raison = "Les utilisateurs suivants seront notifiés de l'ajout du film:";

                // Send email to each user
                //var emailService = new EmailService("smtp.example.com", 587, "your-email@example.com", "your-email-password");
                foreach (var user in usersToNotify)
                {
                    //emailService.SendEmail(user.NomUtilisateur, "Nouveau film ajouté", $"Une film nommé '{filmViewModel.Film.TitreFrancais}' à été ajouté.");
                }

                // Redirect to the Email action with the list of users
                return RedirectToAction("Email", new { emailRaison = raison, filmTitle = filmViewModel.Film.TitreFrancais });
            }
            ViewData["Categorie"] = new SelectList(_context.Categories, "NoCategorie", "Description", filmViewModel.Film.Categorie);
            ViewData["Format"] = new SelectList(_context.Formats, "NoFormat", "Description", filmViewModel.Film.Format);
            ViewData["NoProducteur"] = new SelectList(_context.Producteurs, "NoProducteur", "Nom", filmViewModel.Film.NoProducteur);
            ViewData["NoRealisateur"] = new SelectList(_context.Realisateurs, "NoRealisateur", "Nom", filmViewModel.Film.NoRealisateur);
            ViewData["NoUtilisateurMaj"] = new SelectList(_context.Utilisateurs, "NoUtilisateur", "NomUtilisateur", filmViewModel.Film.NoUtilisateurMaj);
            ViewData["BackgroundImagePath"] = HttpContext.Session.GetString("BackgroundImagePath");
            ViewData["CurrentUser"] = _userIdConnected;

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
            FilmViewModel filmViewModel = new FilmViewModel();
            var film = await _context.Films.FindAsync(id);
            if (film == null)
            {
                return NotFound();
            }

            var utilisateurMaj = await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.NoUtilisateur == film.NoUtilisateurMaj);

            if (utilisateurMaj != null)
                ViewData["NomUtilisateurMaj"] = utilisateurMaj.NomUtilisateur;
            else
                ViewData["NomUtilisateurMaj"] = "Utilisateur non trouvé";


            // Ajouter le type d'utilisateur dans ViewData
            var userType = await GetUserTypeAsync();
            ViewData["UserType"] = userType;

            // Si un utilisateur et pas son dvd retourne a index
            if (userType == "U")
                if (_userIdConnected != film.NoUtilisateurMaj)
                    return Redirect("/Films/Index");

            filmViewModel.Film = film;
            filmViewModel.Film.NoFilm = film.NoFilm;
            ViewData["Categorie"] = new SelectList(_context.Categories, "NoCategorie", "Description", filmViewModel.Film.Categorie);
            ViewData["Format"] = new SelectList(_context.Formats, "NoFormat", "Description", filmViewModel.Film.Format);
            ViewData["NoProducteur"] = new SelectList(_context.Producteurs, "NoProducteur", "Nom", filmViewModel.Film.NoProducteur);
            ViewData["NoRealisateur"] = new SelectList(_context.Realisateurs, "NoRealisateur", "Nom", filmViewModel.Film.NoRealisateur);
            ViewData["NoUtilisateurMaj"] = new SelectList(_context.Utilisateurs, "NoUtilisateur", "NomUtilisateur", filmViewModel.Film.NoUtilisateurMaj);
            ViewData["BackgroundImagePath"] = HttpContext.Session.GetString("BackgroundImagePath");
            ViewData["CurrentUser"] = _userIdConnected;

            return View(filmViewModel);
        }

        // POST: Films/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FilmViewModel filmViewModel)
        {
            if (!IsConnected)
            {
                return Redirect("/Home/Index");
            }
            if (id != filmViewModel.Film.NoFilm)
            {
                return NotFound();
            }
            filmViewModel.Film.DateMaj = DateTime.Now;

            var userType = await GetUserTypeAsync();
            if (userType == "U")
            {
                filmViewModel.Film.NoUtilisateurMaj = _userIdConnected ?? 1;
            }

            ModelState.Remove("ImagePochette");
            ModelState.Remove("Image");
            if (ModelState.IsValid)
            {
                if (filmViewModel.Image != null)
                {
                    string chemin = "wwwroot/images/";
                    //System.IO.File.Delete("wwwroot/images/" + filmViewModel.Film.ImagePochette + ".*");
                    string nomFichier = filmViewModel.Film.NoFilm + Path.GetExtension(filmViewModel.Image.FileName);
                    filmViewModel.Film.ImagePochette = nomFichier;
                    string cheminFichier = chemin + nomFichier;

                    using (var fileStream = new FileStream(cheminFichier, FileMode.Create))
                    {
                        await filmViewModel.Image.CopyToAsync(fileStream);
                    }
                }
                try
                {
                    _context.Update(filmViewModel.Film);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilmExists(filmViewModel.Film.NoFilm))
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
            ViewData["Categorie"] = new SelectList(_context.Categories, "NoCategorie", "Description", filmViewModel.Film.Categorie);
            ViewData["Format"] = new SelectList(_context.Formats, "NoFormat", "Description", filmViewModel.Film.Format);
            ViewData["NoProducteur"] = new SelectList(_context.Producteurs, "NoProducteur", "Nom", filmViewModel.Film.NoProducteur);
            ViewData["NoRealisateur"] = new SelectList(_context.Realisateurs, "NoRealisateur", "Nom", filmViewModel.Film.NoRealisateur);
            ViewData["NoUtilisateurMaj"] = new SelectList(_context.Utilisateurs, "NoUtilisateur", "NomUtilisateur", filmViewModel.Film.NoUtilisateurMaj);
            ViewData["BackgroundImagePath"] = HttpContext.Session.GetString("BackgroundImagePath");
            ViewData["CurrentUser"] = _userIdConnected;

            return View(filmViewModel);
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

            ViewData["BackgroundImagePath"] = HttpContext.Session.GetString("BackgroundImagePath");

            var userType = await GetUserTypeAsync();
            ViewData["UserType"] = userType;
            // Si pas superutilisateur et pas son dvd retourne a index
            if (userType != "S")
                if (_userIdConnected != film.NoUtilisateurMaj)
                    return Redirect("/Films/Index");

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
                await _context.SaveChangesAsync();

                return RedirectToAction("Email", new
                {
                    filmTitle = film.TitreFrancais,
                    emailRaison = "Les utilisateurs suivants seront notifiés de la suppression du film : "
                });
            }
            return NotFound();
        }

        // GET: Films/Approprier/5
        public async Task<IActionResult> Approprier(int? id)
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
            ViewData["BackgroundImagePath"] = HttpContext.Session.GetString("BackgroundImagePath");
            var userType = await GetUserTypeAsync();
            ViewData["UserType"] = userType;

            // Si pas superutilisateur et c'est son dvd retourne a index
            if (userType != "S")
                if (_userIdConnected == film.NoUtilisateurMaj)
                    return Redirect("/Films/Index");

            if (userType == "S")            
                ViewData["NoUtilisateurMaj"] = new SelectList(_context.Utilisateurs, "NoUtilisateur", "NomUtilisateur");         
                
            return View(film);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approprier(int id, int selectedUserId)
        {
            if (!IsConnected)
            {
                return Redirect("/Home/Index");
            }

            var film = await _context.Films.FindAsync(id);
            if (film != null)
            {
                var userType = await GetUserTypeAsync();

                if (userType == "S" && selectedUserId > 0)
                {
                    // Mettre à jour avec l'utilisateur sélectionné
                    film.NoUtilisateurMaj = selectedUserId;
                }
                else
                {
                    // Sinon, assigner l'utilisateur connecté
                    film.NoUtilisateurMaj = _userIdConnected ?? 1;
                }

                _context.Update(film);
                await _context.SaveChangesAsync();
                ViewData["NoUtilisateurMaj"] = new SelectList(_context.Utilisateurs, "NoUtilisateur", "NomUtilisateur", selectedUserId);


                string emailRaison = "Les utilisateurs suivants seront notifiés de l'appropriation du film:";
                return RedirectToAction("Email", new { emailRaison, filmTitle = film.TitreFrancais });
            }

            
            return RedirectToAction(nameof(Index));
        }


        private bool FilmExists(int id)
        {
            return _context.Films.Any(e => e.NoFilm == id);
        }
    }
}
