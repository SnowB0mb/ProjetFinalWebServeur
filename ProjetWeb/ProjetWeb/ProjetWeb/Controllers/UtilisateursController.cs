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
    public class UtilisateursController : Controller
    {
        private readonly FilmDbContext _context;
        public const string SessionKeyId = "_Id";

        private int? _userIdConnected => HttpContext.Session.GetInt32(SessionKeyId);

        public UtilisateursController(FilmDbContext context)
        {
            _context = context;
        }

        // GET: Utilisateurs
        public async Task<IActionResult> Index()
        {
            var filmDbContext = _context.Utilisateurs.Include(u => u.TypeUtilisateurNavigation);
            ViewData["CurrentUser"] = _userIdConnected;

            return View(await filmDbContext.ToListAsync());
        }

        // GET: Utilisateurs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilisateur = await _context.Utilisateurs
                .Include(u => u.TypeUtilisateurNavigation)
                .FirstOrDefaultAsync(m => m.NoUtilisateur == id);
            if (utilisateur == null)
            {
                return NotFound();
            }
            ViewData["CurrentUser"] = _userIdConnected;

            return View(utilisateur);
        }

        // GET: Utilisateurs/Create
        public IActionResult Create()
        {
            ViewData["TypeUtilisateur"] = new SelectList(_context.TypesUtilisateurs.Where(t => t.TypeUtilisateur != "A"), "TypeUtilisateur", "TypeUtilisateur");
            ViewData["CurrentUser"] = _userIdConnected;

            return View();
        }

        // POST: Utilisateurs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NoUtilisateur,NomUtilisateur,Courriel,MotPasse, ConfirmMotPasse, TypeUtilisateur")] Utilisateur utilisateur)
        {
            utilisateur.NoUtilisateur = _context.Utilisateurs.Max(u => u.NoUtilisateur) + 1;

            if (_context.Utilisateurs.Any(u => u.NomUtilisateur == utilisateur.NomUtilisateur))
            {
                ModelState.AddModelError("NomUtilisateur", "Le nom est déjà prit!");
            }
            if (_context.Utilisateurs.Any(u => u.Courriel == utilisateur.Courriel))
            {
                ModelState.AddModelError("Courriel", "Le courriel est déjà prit!");
            }

            if (utilisateur.MotPasse != utilisateur.ConfirmMotPasse)
            {
                ModelState.AddModelError("ConfirmMotPasse", "Les mots de passe ne correspondent pas!");
            }

            if (ModelState.IsValid)
            {
                _context.Add(utilisateur);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TypeUtilisateur"] = new SelectList(_context.TypesUtilisateurs.Where(t => t.TypeUtilisateur != "A"), "TypeUtilisateur", "TypeUtilisateur", utilisateur.TypeUtilisateur);
            ViewData["CurrentUser"] = _userIdConnected;

            return View(utilisateur);
        }

        // GET: Utilisateurs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilisateur = await _context.Utilisateurs.FindAsync(id);
            if (utilisateur == null)
            {
                return NotFound();
            }
            ViewData["TypeUtilisateur"] = new SelectList(_context.TypesUtilisateurs.Where(t => t.TypeUtilisateur != "A"), "TypeUtilisateur", "TypeUtilisateur", utilisateur.TypeUtilisateur);
            ViewData["CurrentUser"] = _userIdConnected;

            return View(utilisateur);
        }

        // POST: Utilisateurs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NoUtilisateur,NomUtilisateur,Courriel,MotPasse,TypeUtilisateur")] Utilisateur utilisateur)
        {
            if (id != utilisateur.NoUtilisateur)
            {
                return NotFound();
            }

            if (_context.Utilisateurs.Any(u => u.NomUtilisateur == utilisateur.NomUtilisateur && u.NoUtilisateur != id))
            {
                ModelState.AddModelError("NomUtilisateur", "Le nom est déjà prit!");
            }
            if (_context.Utilisateurs.Any(u => u.Courriel == utilisateur.Courriel && u.NoUtilisateur != id))
            {
                ModelState.AddModelError("Courriel", "Le courriel est déjà prit!");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(utilisateur);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UtilisateurExists(utilisateur.NoUtilisateur))
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
            ViewData["TypeUtilisateur"] = new SelectList(_context.TypesUtilisateurs.Where(t => t.TypeUtilisateur != "A"), "TypeUtilisateur", "TypeUtilisateur", utilisateur.TypeUtilisateur);
            ViewData["CurrentUser"] = _userIdConnected;

            return View(utilisateur);
        }

        // GET: Utilisateurs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilisateur = await _context.Utilisateurs
                .Include(u => u.TypeUtilisateurNavigation)
                .FirstOrDefaultAsync(m => m.NoUtilisateur == id);
            if (utilisateur == null)
            {
                return NotFound();
            }
            ViewData["CurrentUser"] = _userIdConnected;

            return View(utilisateur);
        }

        // POST: Utilisateurs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var utilisateur = await _context.Utilisateurs.FindAsync(id);
            if (utilisateur != null)
            {
                _context.Utilisateurs.Remove(utilisateur);
            }

            await _context.SaveChangesAsync();
            ViewData["CurrentUser"] = _userIdConnected;

            return RedirectToAction(nameof(Index));
        }

        private bool UtilisateurExists(int id)
        {
            return _context.Utilisateurs.Any(e => e.NoUtilisateur == id);
        }
    }
}
