using Quarto__Mese_BW.Models;
using System.Collections.Generic;

namespace Quarto__Mese_BW.ViewModels
{
    public class OrdineDettaglioViewModel
    {
        public Ordine Ordine { get; set; }
        public Anagrafica Anagrafica { get; set; }
        public List<ProdottoOrdine> Prodotti { get; set; }
    }
}
