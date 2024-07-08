using System.Collections.Generic;
using System.Linq;
using Quarto__Mese_BW.Models;

namespace Quarto__Mese_BW.Services
{
    public class CarrelloService
    {
        private readonly Dictionary<int, (Prodotto Prodotto, int Quantità)> _carrello = new Dictionary<int, (Prodotto, int)>();

        public void AggiungiAlCarrello(Prodotto prodotto, int quantità = 1)
        {
            if (_carrello.ContainsKey(prodotto.ProductID))
            {
                _carrello[prodotto.ProductID] = (_carrello[prodotto.ProductID].Prodotto, _carrello[prodotto.ProductID].Quantità + quantità);
            }
            else
            {
                _carrello[prodotto.ProductID] = (prodotto, quantità);
            }
        }

        public void RimuoviDalCarrello(int productId)
        {
            if (_carrello.ContainsKey(productId))
            {
                _carrello.Remove(productId);
            }
        }

        public void SvuotaCarrello()
        {
            _carrello.Clear();
        }

        public void AggiornaQuantità(int productId, int quantità)
        {
            if (_carrello.ContainsKey(productId))
            {
                var prodotto = _carrello[productId].Prodotto;
                _carrello[productId] = (prodotto, quantità);
            }
        }

        public IEnumerable<(Prodotto Prodotto, int Quantità)> GetCarrelloProdotti()
        {
            return _carrello.Values;
        }

        public bool IsEmpty()
        {
            return !_carrello.Any();
        }

        public int GetNumeroProdotti()
        {
            return _carrello.Sum(item => item.Value.Quantità);
        }
    }
}
