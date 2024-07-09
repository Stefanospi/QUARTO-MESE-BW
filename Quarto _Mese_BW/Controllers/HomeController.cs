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

        public IActionResult Dettagli(int id)
        {
            var prodotto = _prodottoService.GetProdottoById(id);
            return View(prodotto);
        }

        [HttpPost]
        public IActionResult Aggiungi(int productId, int quantità = 1)
        {
            _carrelloService.AggiungiAlCarrello(productId, quantità);
            ViewBag.NumeroProdotti = _carrelloService.GetNumeroProdotti();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Rimuovi(int productId)
        {
            _carrelloService.RimuoviDalCarrello(productId);
            ViewBag.NumeroProdotti = _carrelloService.GetNumeroProdotti();
            return RedirectToAction("Visualizza");
        }

        [HttpPost]
        public IActionResult AggiornaQuantità(int productId, int quantità)
        {
            _carrelloService.AggiornaQuantità(productId, quantità);
            ViewBag.NumeroProdotti = _carrelloService.GetNumeroProdotti();
            return RedirectToAction("Visualizza");
        }

        [HttpPost]
        public IActionResult SvuotaCarrello()
        {
            _carrelloService.SvuotaCarrello();
            ViewBag.NumeroProdotti = _carrelloService.GetNumeroProdotti();
            return RedirectToAction("Visualizza");
        }

        public IActionResult Visualizza()
        {
            var carrelloItems = _carrelloService.GetCarrelloProdotti();
            var prodotti = carrelloItems.Select(item => (item.Prodotto, item.Quantità));
            ViewBag.NumeroProdotti = _carrelloService.GetNumeroProdotti();
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
