namespace Quarto__Mese_BW.Models
{
    public class Prodotto
    {
        public int ProductID { get; set; }
        public string Nome { get; set; }
        public string Descrizione { get; set; }
        public decimal Prezzo { get; set; }
        public string ImmagineUrl { get; set; }
        public int Stock { get; set; }
        public int CategoriaID { get; set; }
    }
}
