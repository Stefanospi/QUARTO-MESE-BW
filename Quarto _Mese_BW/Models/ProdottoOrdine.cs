namespace Quarto__Mese_BW.Models
{
    public class ProdottoOrdine
    {
        public int ProductID { get; set; }
        public int Quantità { get; set; }
        public decimal PrezzoUnitario { get; set; }
        public string Nome { get; set; } // Aggiungi questa proprietà
        public string Descrizione { get; set; } // Aggiungi questa proprietà
        public string ImmagineUrl { get; set; } // Aggiungi questa proprietà se necessario
    }
}
