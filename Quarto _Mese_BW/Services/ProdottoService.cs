using System.Collections.Generic;
using System.Data.SqlClient;
using Quarto__Mese_BW.Models;
using System.Data.Common;

namespace Quarto__Mese_BW.Services
{
    public class ProdottoService : SqlServerServiceBase, IProdottoService
    {
        public ProdottoService(IConfiguration config) : base(config) { }

        public IEnumerable<Prodotto> GetAllProdotti()
        {
            var prodotti = new List<Prodotto>();
            using var conn = GetConnection();
            conn.Open();
            using var cmd = GetCommand("SELECT ProductID, Nome, Descrizione, Prezzo, ImmagineUrl, Stock, CategoriaID FROM Prodotti");
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                prodotti.Add(new Prodotto
                {
                    ProductID = reader.GetInt32(0),
                    Nome = reader.GetString(1),
                    Descrizione = reader.GetString(2),
                    Prezzo = reader.GetDecimal(3),
                    ImmagineUrl = reader.IsDBNull(4) ? null : reader.GetString(4),
                    Stock = reader.GetInt32(5),
                    CategoriaID = reader.GetInt32(6)
                });
            }
            return prodotti;
        }

        public Prodotto GetProdottoById(int id)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = GetCommand("SELECT ProductID, Nome, Descrizione, Prezzo, ImmagineUrl, Stock, CategoriaID FROM Prodotti WHERE ProductID = @id");
            cmd.Parameters.Add(new SqlParameter("@id", id));
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Prodotto
                {
                    ProductID = reader.GetInt32(0),
                    Nome = reader.GetString(1),
                    Descrizione = reader.GetString(2),
                    Prezzo = reader.GetDecimal(3),
                    ImmagineUrl = reader.IsDBNull(4) ? null : reader.GetString(4),
                    Stock = reader.GetInt32(5),
                    CategoriaID = reader.GetInt32(6)
                };
            }
            return null;
        }

        public void AddProdotto(Prodotto prodotto)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = GetCommand("INSERT INTO Prodotti (Nome, Descrizione, Prezzo, ImmagineUrl, Stock, CategoriaID) VALUES (@Nome, @Descrizione, @Prezzo, @ImmagineUrl, @Stock, @CategoriaID)");
            cmd.Parameters.Add(new SqlParameter("@Nome", prodotto.Nome));
            cmd.Parameters.Add(new SqlParameter("@Descrizione", prodotto.Descrizione));
            cmd.Parameters.Add(new SqlParameter("@Prezzo", prodotto.Prezzo));
            cmd.Parameters.Add(new SqlParameter("@ImmagineUrl", string.IsNullOrEmpty(prodotto.ImmagineUrl) ? (object)DBNull.Value : prodotto.ImmagineUrl));
            cmd.Parameters.Add(new SqlParameter("@Stock", prodotto.Stock));
            cmd.Parameters.Add(new SqlParameter("@CategoriaID", prodotto.CategoriaID));
            cmd.ExecuteNonQuery();
        }

        public void UpdateProdotto(Prodotto prodotto)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = GetCommand("UPDATE Prodotti SET Nome = @Nome, Descrizione = @Descrizione, Prezzo = @Prezzo, ImmagineUrl = @ImmagineUrl, Stock = @Stock, CategoriaID = @CategoriaID WHERE ProductID = @ProductID");
            cmd.Parameters.Add(new SqlParameter("@Nome", prodotto.Nome));
            cmd.Parameters.Add(new SqlParameter("@Descrizione", prodotto.Descrizione));
            cmd.Parameters.Add(new SqlParameter("@Prezzo", prodotto.Prezzo));
            cmd.Parameters.Add(new SqlParameter("@ImmagineUrl", string.IsNullOrEmpty(prodotto.ImmagineUrl) ? (object)DBNull.Value : prodotto.ImmagineUrl));
            cmd.Parameters.Add(new SqlParameter("@Stock", prodotto.Stock));
            cmd.Parameters.Add(new SqlParameter("@CategoriaID", prodotto.CategoriaID));
            cmd.Parameters.Add(new SqlParameter("@ProductID", prodotto.ProductID));
            cmd.ExecuteNonQuery();
        }

        public void DeleteProdotto(int id)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = GetCommand("DELETE FROM Prodotti WHERE ProductID = @id");
            cmd.Parameters.Add(new SqlParameter("@id", id));
            cmd.ExecuteNonQuery();
        }
    }
}
