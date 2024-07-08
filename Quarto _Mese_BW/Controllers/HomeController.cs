using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Quarto__Mese_BW.Models;
using Quarto__Mese_BW.Services;

namespace Quarto__Mese_BW.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProdottoService _prodottoService;
        private readonly CarrelloService _carrelloService;
        private readonly IAuthService _authService;

        public HomeController(IProdottoService prodottoService, CarrelloService carrelloService, IAuthService authService)
        {
            _prodottoService = prodottoService;
            _carrelloService = carrelloService;
            _authService = authService;
        }

        public IActionResult Index()
        {
            var prodotti = _prodottoService.GetAllProdotti();
            ViewBag.NumeroProdotti = _carrelloService.GetNumeroProdotti();
            return View(prodotti);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Aggiungi(int productId, int quantità = 1)
        {
            var prodotto = _prodottoService.GetAllProdotti().FirstOrDefault(p => p.ProductID == productId);
            if (prodotto != null)
            {
                _carrelloService.AggiungiAlCarrello(prodotto, quantità);
            }
            ViewBag.NumeroProdotti = _carrelloService.GetNumeroProdotti(); // Aggiorna il numero di prodotti
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Rimuovi(int productId)
        {
            _carrelloService.RimuoviDalCarrello(productId);
            ViewBag.NumeroProdotti = _carrelloService.GetNumeroProdotti(); // Aggiorna il numero di prodotti
            return RedirectToAction("Visualizza");
        }

        [HttpPost]
        public IActionResult AggiornaQuantità(int productId, int quantità)
        {
            _carrelloService.AggiornaQuantità(productId, quantità);
            ViewBag.NumeroProdotti = _carrelloService.GetNumeroProdotti(); // Aggiorna il numero di prodotti
            return RedirectToAction("Visualizza");
        }

        [HttpPost]
        public IActionResult SvuotaCarrello()
        {
            _carrelloService.SvuotaCarrello();
            ViewBag.NumeroProdotti = _carrelloService.GetNumeroProdotti(); // Aggiorna il numero di prodotti
            return RedirectToAction("Visualizza");
        }

        public IActionResult Visualizza()
        {
            var prodotti = _carrelloService.GetCarrelloProdotti();
            ViewBag.NumeroProdotti = _carrelloService.GetNumeroProdotti(); // Aggiorna il numero di prodotti
            return View(prodotti);
        }

        public IActionResult Acquista()
        {
            if (!_authService.IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            if (_carrelloService.IsEmpty())
            {
                ModelState.AddModelError("", "Il carrello è vuoto!");
                return View("Visualizza", _carrelloService.GetCarrelloProdotti());
            }

            // Logica per l'acquisto

            return View("ConfermaAcquisto");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
