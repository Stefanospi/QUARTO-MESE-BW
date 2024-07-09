using System.Collections.Generic;
using Quarto__Mese_BW.Models;

namespace Quarto__Mese_BW.Services
{
    public interface IProdottoService
    {
        IEnumerable<Prodotto> GetAllProdotti();
        Prodotto GetProdottoById(int id);
    }
}
