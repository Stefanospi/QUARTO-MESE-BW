using System;
using System.Collections.Generic;

namespace Quarto__Mese_BW.Models
{
    public class Ordine
    {
        public int OrderID { get; set; }
        public int UserID { get; set; }
        public DateTime DataOrdine { get; set; }
        public string Stato { get; set; }
        public decimal Totale { get; set; }
        public List<ProdottoOrdine> Prodotti { get; set; }
    }
}
