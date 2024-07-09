using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Quarto__Mese_BW.Models;
using Quarto__Mese_BW.Services;

namespace Quarto__Mese_BW.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProdottoService _prodottoService;

    public HomeController(IProdottoService prodottoService)
    {
        _prodottoService = prodottoService;
    }

    public IActionResult Index()
    {
        var prodotti = _prodottoService.GetAllProdotti();
        return View(prodotti);
    }

    public IActionResult Dettagli(int id)
    {
        var prodotto = _prodottoService.GetProdottoById(id);
        //var categoria = _prodottoService.GetCategoriaNomeById(prodotto.CategoriaID);
        //ViewData["Categoria"] = categoria;
        return View(prodotto);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

