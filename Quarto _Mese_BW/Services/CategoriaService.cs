using System.Collections.Generic;
using System.Data.SqlClient;
using Quarto__Mese_BW.Models;
using System.Data.Common;

namespace Quarto__Mese_BW.Services
{
    public class CategoriaService : SqlServerServiceBase, ICategoriaService
    {
        public CategoriaService(IConfiguration config) : base(config) { }

        public IEnumerable<Categoria> GetAllCategorie()
        {
            var categorie = new List<Categoria>();
            using var conn = GetConnection();
            conn.Open();
            using var cmd = GetCommand("SELECT CategoriaID, NomeCategoria FROM Categorie");
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                categorie.Add(new Categoria
                {
                    CategoriaID = reader.GetInt32(0),
                    NomeCategoria = reader.GetString(1)
                });
            }
            return categorie;
        }
    }
}
