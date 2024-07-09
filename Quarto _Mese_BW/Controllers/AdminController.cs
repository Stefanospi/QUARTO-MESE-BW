using Microsoft.AspNetCore.Mvc;
using Quarto__Mese_BW.Models;
using Quarto__Mese_BW.Services;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Quarto__Mese_BW.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IProdottoService _prodottoService;
        private readonly ICategoriaService _categoriaService;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public AdminController(IAuthService authService, IProdottoService prodottoService, ICategoriaService categoriaService, IWebHostEnvironment hostingEnvironment)
        {
            _authService = authService;
            _prodottoService = prodottoService;
            _categoriaService = categoriaService;
            _hostingEnvironment = hostingEnvironment; // Corretto il nome della variabile qui
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

            var model = new ProductInputModel
            {
                ProductID = prodotto.ProductID,
                Nome = prodotto.Nome,
                Descrizione = prodotto.Descrizione,
                Prezzo = prodotto.Prezzo,
                ImmagineUrl = prodotto.ImmagineUrl,
                Stock = prodotto.Stock,
                CategoriaID = prodotto.CategoriaID
            };

            var categorie = _categoriaService.GetAllCategorie().ToList();
            ViewBag.Categorie = categorie;

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(ProductInputModel model)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Login");
            }

            if (ModelState.IsValid)
            {
                var prodotto = new Prodotto
                {
                    ProductID = model.ProductID,
                    Nome = model.Nome,
                    Descrizione = model.Descrizione,
                    Prezzo = model.Prezzo,
                    ImmagineUrl = model.ImmagineUrl,
                    Stock = model.Stock,
                    CategoriaID = model.CategoriaID
                };

                try
                {
                    if (model.ImmagineFile != null && model.ImmagineFile.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "img");
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImmagineFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            model.ImmagineFile.CopyTo(fileStream);
                        }

                        prodotto.ImmagineUrl = "/img/" + uniqueFileName;
                    }

                    _prodottoService.UpdateProdotto(prodotto);
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Si è verificato un errore durante il salvataggio del prodotto: " + ex.Message);
                }
            }

            var categorie = _categoriaService.GetAllCategorie().ToList();
            ViewBag.Categorie = categorie;

            return View(model);
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


        public IActionResult Create()
        {
            var categorie = _categoriaService.GetAllCategorie();
            ViewBag.Categorie = new SelectList(categorie, "CategoriaID", "NomeCategoria");

            return View();
        }

        [HttpPost]
        public IActionResult Create(ProductInputModel model)
        {
            if (ModelState.IsValid)
            {
                var prodotto = new Prodotto
                {
                    Nome = model.Nome,
                    Descrizione = model.Descrizione,
                    Prezzo = model.Prezzo,
                    Stock = model.Stock,
                    CategoriaID = model.CategoriaID
                };

                try
                {
                    if (model.ImmagineFile != null && model.ImmagineFile.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "img");
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImmagineFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            model.ImmagineFile.CopyTo(fileStream);
                        }

                        prodotto.ImmagineUrl = "/img/" + uniqueFileName;
                    }

                    _prodottoService.AddProdotto(prodotto);
                    return RedirectToAction("Index", "Home"); // Modifica se necessario
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Si è verificato un errore durante il salvataggio del prodotto: " + ex.Message);
                }
            }

            var categorie = _categoriaService.GetAllCategorie();
            ViewBag.Categorie = new SelectList(categorie, "CategoriaID", "NomeCategoria");

            return View(model);
        }

    }
}
