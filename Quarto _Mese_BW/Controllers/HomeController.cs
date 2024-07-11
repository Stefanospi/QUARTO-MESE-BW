using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Quarto__Mese_BW.Models;
using Quarto__Mese_BW.Services;
using Quarto__Mese_BW.ViewModels;
using Microsoft.AspNetCore.Http;

namespace Quarto__Mese_BW.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProdottoService _prodottoService;
        private readonly CarrelloService _carrelloService;
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserService> _userServiceLogger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(IProdottoService prodottoService, CarrelloService carrelloService, IAuthService authService, IConfiguration configuration, ILogger<HomeController> logger, ILogger<UserService> userServiceLogger, IHttpContextAccessor httpContextAccessor)
        {
            _prodottoService = prodottoService;
            _carrelloService = carrelloService;
            _authService = authService;
            _configuration = configuration;
            _logger = logger;
            _userServiceLogger = userServiceLogger;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult RiepilogoCarrello()
        {
            var carrelloItems = _carrelloService.GetCarrelloProdotti();
            var user = new Anagrafica(); // Initialize an empty Anagrafica object

            var modello = new RiepilogoCarrelloViewModel
            {
                Prodotti = carrelloItems.Select(item => (item.Prodotto, item.Quantità)),
                Anagrafica = user
            };
            return View(modello);
        }

        public IActionResult Index()
        {
            var prodotti = _prodottoService.GetAllProdotti();
            ViewBag.NumeroProdotti = _carrelloService.GetNumeroProdotti();
            return View(prodotti);
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
            ViewBag.CarrelloVuoto = _carrelloService.IsEmpty();
            return View(prodotti);
        }

        [HttpPost]
        public IActionResult EliminaOrdine(int orderId)
        {
            _carrelloService.EliminaOrdine(orderId);
            return RedirectToAction("Ordini");
        }

        [HttpPost]
        public IActionResult Acquista(Anagrafica anagrafica)
        {
            if (_carrelloService.IsEmpty())
            {
                TempData["CarrelloVuoto"] = true;
                return RedirectToAction("Visualizza");
            }

            if (ModelState.IsValid)
            {
                _carrelloService.CompletaAcquisto(anagrafica);
                return View("ConfermaAcquisto");
            }

            var carrelloItems = _carrelloService.GetCarrelloProdotti();
            var prodotti = carrelloItems.Select(item => (item.Prodotto, item.Quantità));
            var modello = new RiepilogoCarrelloViewModel
            {
                Prodotti = prodotti.ToList(),
                Anagrafica = anagrafica
            };
            return View("RiepilogoCarrello", modello);
        }

        public IActionResult Ordini()
        {
            var ordini = _carrelloService.GetOrdini();
            return View(ordini);
        }

        public IActionResult DettagliOrdine(int id)
        {
            var ordineDettaglio = _carrelloService.GetOrdineDettaglioById(id);
            return View(ordineDettaglio);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult AggiornaQuantitàProdottoOrdine(int orderId, int productId, int quantità)
        {
            _carrelloService.AggiornaQuantitàProdottoOrdine(orderId, productId, quantità);
            return RedirectToAction("DettagliOrdine", new { id = orderId });
        }

        [HttpPost]
        public IActionResult EliminaProdottoOrdine(int orderId, int productId)
        {
            _carrelloService.EliminaProdottoOrdine(orderId, productId);
            return RedirectToAction("DettagliOrdine", new { id = orderId });
        }

        [HttpPost]
        public IActionResult SalvaAnagrafica(Anagrafica anagrafica)
        {
            if (ModelState.IsValid)
            {
                _carrelloService.SalvaAnagrafica(anagrafica);
                TempData["AnagraficaSalvata"] = true;
            }
            return RedirectToAction("RiepilogoCarrello");
        }
    }
}
