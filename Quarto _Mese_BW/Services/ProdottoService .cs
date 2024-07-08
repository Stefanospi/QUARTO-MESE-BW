using System.Collections.Generic;
using System.Data;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Quarto__Mese_BW.Models;

namespace Quarto__Mese_BW.Services
{
    public class ProdottoService : IProdottoService
    {
        private readonly string _connectionString;

        public ProdottoService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ECommerce");
        }

        public IEnumerable<Prodotto> GetAllProdotti()
        {
            var prodotti = new List<Prodotto>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT p.ProductID, p.Nome, p.Descrizione, p.Prezzo, p.ImmagineUrl, p.Stock, p.CategoriaID, c.NomeCategoria FROM Prodotti p JOIN Categorie c ON p.CategoriaID = c.CategoriaID";
                var command = new SqlCommand(query, connection);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var prodotto = new Prodotto
                        {
                            ProductID = reader.GetInt32(0),
                            Nome = reader.GetString(1),
                            Descrizione = reader.GetString(2),
                            Prezzo = reader.GetDecimal(3),
                            ImmagineUrl = reader.GetString(4),
                            Stock = reader.GetInt32(5),
                            CategoriaID = reader.GetInt32(6),
                            Categoria = new Categoria
                            {
                                CategoriaID = reader.GetInt32(6),
                                NomeCategoria = reader.GetString(7)
                            }
                        };
                        prodotti.Add(prodotto);
                    }
                }
            }

            return prodotti;
        }
        public Prodotto GetProdottoById(int id)
        {
            var prodotto = new Prodotto();
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT p.ProductID, p.Nome, p.Descrizione, p.Prezzo, p.ImmagineUrl, p.Stock, p.CategoriaID, c.NomeCategoria FROM Prodotti p JOIN Categorie c ON p.CategoriaID = c.CategoriaID WHERE p.ProductID = @ProductID";
                var command = new SqlCommand(query, connection);
                command.Parameters.Add("@ProductID", SqlDbType.Int).Value = id;

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var prodottoId = new Prodotto
                        {
                            ProductID = reader.GetInt32(0),
                            Nome = reader.GetString(1),
                            Descrizione = reader.GetString(2),
                            Prezzo = reader.GetDecimal(3),
                            ImmagineUrl = reader.GetString(4),
                            Stock = reader.GetInt32(5),
                            CategoriaID = reader.GetInt32(6),
                            Categoria = new Categoria
                            {
                                CategoriaID = reader.GetInt32(6),
                                NomeCategoria = reader.GetString(7)
                            }
                        };
                      prodotto = prodottoId;
                    }
                }
            }
            return prodotto;
        }
        
    }
}