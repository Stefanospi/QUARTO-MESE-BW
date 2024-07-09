using Microsoft.AspNetCore.Mvc;
using Quarto__Mese_BW.Models;
using Quarto__Mese_BW.Services;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Quarto__Mese_BW.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IProdottoService _prodottoService;
        private readonly ICategoriaService _categoriaService;

        public AdminController(IAuthService authService, IProdottoService prodottoService, ICategoriaService categoriaService)
        {
            _authService = authService;
            _prodottoService = prodottoService;
            _categoriaService = categoriaService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (_authService.Authenticate(email, password))
            {
                HttpContext.Session.SetString("IsAdmin", "true");
                return RedirectToAction("Index");
            }

            ViewBag.ErrorMessage = "Tentativo di accesso non valido";
            return View();
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Login");
            }

            var prodotti = _prodottoService.GetAllProdotti();

            return View(prodotti);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("IsAdmin");
            return RedirectToAction("Login");
        }

        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Login");
            }

            var prodotto = _prodottoService.GetProdottoById(id);
            if (prodotto == null)
            {
                return NotFound();
            }

            var categorie = _categoriaService.GetAllCategorie().ToList();
            ViewBag.Categorie = categorie;

            return View(prodotto);
        }

        [HttpPost]
        public IActionResult Edit(Prodotto prodotto)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Login");
            }

            if (ModelState.IsValid)
            {
                _prodottoService.UpdateProdotto(prodotto);
                return RedirectToAction("Index");
            }

            var categorie = _categoriaService.GetAllCategorie().ToList();
            ViewBag.Categorie = categorie;

            return View(prodotto);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return Json(new { success = false, message = "Non autorizzato" });
            }

            _prodottoService.DeleteProdotto(id);
            return Json(new { success = true });
        }
    }
}
