using Quarto__Mese_BW.Models;
using System.Collections.Generic;

namespace Quarto__Mese_BW.Services
{
    public interface ICategoriaService
    {
        IEnumerable<Categoria> GetAllCategorie();
    }
}
