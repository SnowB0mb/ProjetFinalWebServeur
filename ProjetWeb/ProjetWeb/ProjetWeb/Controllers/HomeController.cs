using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ProjetWeb.Models;
using System.Diagnostics;

namespace ProjetWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FilmDbContext _context;

        public HomeController(ILogger<HomeController> logger, FilmDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index([Bind("NomUtilisateur, MotPasse")] Utilisateur utilisateur)
        {
            //fermer la session de l'utilisateur si elle existe
            var utilisateurDbContext = _context.Utilisateurs.Where(u => u.NomUtilisateur == utilisateur.NomUtilisateur && u.MotPasse == utilisateur.MotPasse).FirstOrDefault();
            if (utilisateurDbContext == null)
            {
                ModelState.AddModelError("MotPasse", "Nom d'utilisateur ou mot de passe incorrect");
                return View(utilisateur);
            }
            return Redirect("/Films/Index");
        }

        public IActionResult Inscription()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Inscription(Utilisateur utilisateur)
        {
            utilisateur.NoUtilisateur = _context.Utilisateurs.Max(u => u.NoUtilisateur) + 1;
            //l'inscription crée toujours un utilisateur de type "U" Utilisateur
            utilisateur.TypeUtilisateur = "U";
            ModelState.Remove("TypeUtilisateur");
            var utilisateurNomDbContext = _context.Utilisateurs.Where(u => u.NomUtilisateur == utilisateur.NomUtilisateur).FirstOrDefault();
            var utilisateurCourrielDbContext = _context.Utilisateurs.Where(u => u.Courriel == utilisateur.Courriel).FirstOrDefault();
            if (utilisateurNomDbContext != null)
            {
                ModelState.AddModelError("NomUtilisateur", "Ce nom d'utilisateur est déjà utilisé");
            }
            if (utilisateurCourrielDbContext != null)
            {
                ModelState.AddModelError("Courriel", "Ce courriel est déjà utilisé");
            }

            if (!ModelState.IsValid)
            {
                return View(utilisateur);
            }
            _context.Add(utilisateur);
            _context.SaveChanges();
            return Redirect("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
