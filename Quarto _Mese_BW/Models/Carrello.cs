namespace Quarto__Mese_BW.Models
{
    public class Carrello
    {
        public int CartID { get; set; }
        public int UserID { get; set; }
        public DateTime DataCreazione { get; set; }
        public List<ProdottiCarrello> Prodotti { get; set; }
    }
}
