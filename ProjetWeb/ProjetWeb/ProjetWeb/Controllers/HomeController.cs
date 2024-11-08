using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public IActionResult Index([Bind("NomUtilisateur, MotPasse")]Utilisateur utilisateur)
        {
            var utilisateurDbContext = _context.Utilisateurs.Where(u => u.NomUtilisateur == utilisateur.NomUtilisateur && u.MotPasse == utilisateur.MotPasse).FirstOrDefault();
            if(utilisateurDbContext == null)
            {
                ModelState.AddModelError("MotPasse", "Nom d'utilisateur ou mot de passe incorrect");
                return View(utilisateur);
            }
            return Redirect("Films/Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
