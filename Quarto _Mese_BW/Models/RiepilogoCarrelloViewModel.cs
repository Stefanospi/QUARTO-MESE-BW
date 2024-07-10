using System.Collections.Generic;
using Quarto__Mese_BW.Models;

namespace Quarto__Mese_BW.ViewModels
{
    public class RiepilogoCarrelloViewModel
    {
        public IEnumerable<(Prodotto Prodotto, int Quantità)> Prodotti { get; set; }
        public Anagrafica Anagrafica { get; set; }
    }
}
