using Quarto__Mese_BW.Models;
using System.Collections.Generic;

namespace Quarto__Mese_BW.Services
{
    public interface IProdottoService
    {
        IEnumerable<Prodotto> GetAllProdotti();
        Prodotto GetProdottoById(int id);
        string GetCategoriaNomeById(int id); // New method to get category name by id
        void AddProdotto(Prodotto prodotto);
        void UpdateProdotto(Prodotto prodotto);
        void DeleteProdotto(int id);
    }
}
