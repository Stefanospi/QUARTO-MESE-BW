namespace Quarto__Mese_BW.Models
{
    public class ProdottiCarrello
    {
        public int ProdottiCarrelloID { get; set; }
        public int CartID { get; set; }
        public int ProductID { get; set; }
        public int Quantità { get; set; }
        public Prodotto Prodotto { get; set; }
    }
}
