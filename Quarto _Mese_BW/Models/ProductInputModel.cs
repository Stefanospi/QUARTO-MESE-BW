using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Quarto__Mese_BW.Models
{
    public class ProductInputModel
    {
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Il nome del prodotto è obbligatorio")]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "La descrizione del prodotto è obbligatoria")]
        [Display(Name = "Descrizione")]
        public string Descrizione { get; set; }

        [Required(ErrorMessage = "Il prezzo del prodotto è obbligatorio")]
        [Display(Name = "Prezzo")]
        public decimal Prezzo { get; set; }

        [Display(Name = "Immagine esistente")]
        public string ? ImmagineUrl { get; set; }

        [Display(Name = "Nuova Immagine")]
        public IFormFile ImmagineFile { get; set; }

        [Required(ErrorMessage = "Il campo Quantità in magazzino è obbligatorio")]
        [Display(Name = "Quantità in magazzino")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Seleziona una categoria")]
        [Display(Name = "Categoria")]
        public int CategoriaID { get; set; }
    }
}
