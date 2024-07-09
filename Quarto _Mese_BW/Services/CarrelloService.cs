using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Quarto__Mese_BW.Models;

namespace Quarto__Mese_BW.Services
{
    public class CarrelloService
    {
        private readonly string _connectionString;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string SessionKeyCartId = "CartId";

        public CarrelloService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _connectionString = configuration.GetConnectionString("ECommerce");
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetUserId()
        {
            // Recupera l'ID utente corrente
            var userIdString = _httpContextAccessor.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdString, out var userId) ? userId : 0;
        }

        private int GetOrCreateCarrelloId()
        {
            // Controlla se c'è già un carrello ID nella sessione
            var cartIdSession = _httpContextAccessor.HttpContext.Session.GetInt32(SessionKeyCartId);

            if (cartIdSession.HasValue)
            {
                return cartIdSession.Value;
            }

            var userId = GetUserId();
            int cartId;

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT CartID FROM Carrello WHERE UserID = @UserID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserID", userId == 0 ? DBNull.Value : (object)userId);

                var cartIdObj = command.ExecuteScalar();
                if (cartIdObj == DBNull.Value || cartIdObj == null)
                {
                    query = "INSERT INTO Carrello (UserID, DataCreazione) OUTPUT INSERTED.CartID VALUES (@UserID, @DataCreazione)";
                    command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@UserID", userId == 0 ? DBNull.Value : (object)userId);
                    command.Parameters.AddWithValue("@DataCreazione", DateTime.Now);

                    cartId = (int)command.ExecuteScalar();
                }
                else
                {
                    cartId = (int)cartIdObj;
                }

                // Salva il cart ID nella sessione
                _httpContextAccessor.HttpContext.Session.SetInt32(SessionKeyCartId, cartId);
            }

            return cartId;
        }

        public void AggiungiAlCarrello(int productId, int quantità = 1)
        {
            var cartId = GetOrCreateCarrelloId();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT ProdottiCarrelloID, Quantità FROM ProdottiCarrello WHERE CartID = @CartID AND ProductID = @ProductID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CartID", cartId);
                command.Parameters.AddWithValue("@ProductID", productId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var prodottiCarrelloId = reader.GetInt32(0);
                        var quantitàEsistente = reader.GetInt32(1);

                        reader.Close();
                        query = "UPDATE ProdottiCarrello SET Quantità = @Quantità WHERE ProdottiCarrelloID = @ProdottiCarrelloID";
                        command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@Quantità", quantitàEsistente + quantità);
                        command.Parameters.AddWithValue("@ProdottiCarrelloID", prodottiCarrelloId);
                        command.ExecuteNonQuery();
                    }
                    else
                    {
                        reader.Close();
                        query = "INSERT INTO ProdottiCarrello (CartID, ProductID, Quantità) VALUES (@CartID, @ProductID, @Quantità)";
                        command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@CartID", cartId);
                        command.Parameters.AddWithValue("@ProductID", productId);
                        command.Parameters.AddWithValue("@Quantità", quantità);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public void RimuoviDalCarrello(int productId)
        {
            var cartId = GetOrCreateCarrelloId();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "DELETE FROM ProdottiCarrello WHERE CartID = @CartID AND ProductID = @ProductID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CartID", cartId);
                command.Parameters.AddWithValue("@ProductID", productId);
                command.ExecuteNonQuery();
            }
        }

        public void SvuotaCarrello()
        {
            var cartId = GetOrCreateCarrelloId();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "DELETE FROM ProdottiCarrello WHERE CartID = @CartID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CartID", cartId);
                command.ExecuteNonQuery();
            }
        }

        public void AggiornaQuantità(int productId, int quantità)
        {
            var cartId = GetOrCreateCarrelloId();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "UPDATE ProdottiCarrello SET Quantità = @Quantità WHERE CartID = @CartID AND ProductID = @ProductID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CartID", cartId);
                command.Parameters.AddWithValue("@ProductID", productId);
                command.Parameters.AddWithValue("@Quantità", quantità);
                command.ExecuteNonQuery();
            }
        }

        public IEnumerable<ProdottiCarrello> GetCarrelloProdotti()
        {
            var cartId = GetOrCreateCarrelloId();
            var items = new List<ProdottiCarrello>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT pc.ProdottiCarrelloID, pc.CartID, pc.ProductID, pc.Quantità, p.Nome, p.Descrizione, p.Prezzo, p.ImmagineUrl, p.Stock, p.CategoriaID FROM ProdottiCarrello pc JOIN Prodotti p ON pc.ProductID = p.ProductID WHERE pc.CartID = @CartID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CartID", cartId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new ProdottiCarrello
                        {
                            ProdottiCarrelloID = reader.GetInt32(0),
                            CartID = reader.GetInt32(1),
                            ProductID = reader.GetInt32(2),
                            Quantità = reader.GetInt32(3),
                            Prodotto = new Prodotto
                            {
                                ProductID = reader.GetInt32(2),
                                Nome = reader.GetString(4),
                                Descrizione = reader.GetString(5),
                                Prezzo = reader.GetDecimal(6),
                                ImmagineUrl = reader.GetString(7),
                                Stock = reader.GetInt32(8),
                                CategoriaID = reader.GetInt32(9)
                            }
                        };
                        items.Add(item);
                    }
                }
            }

            return items;
        }

        public bool IsEmpty()
        {
            var cartId = GetOrCreateCarrelloId();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT COUNT(*) FROM ProdottiCarrello WHERE CartID = @CartID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CartID", cartId);
                var count = (int)command.ExecuteScalar();
                return count == 0;
            }
        }

        public int GetNumeroProdotti()
        {
            var cartId = GetOrCreateCarrelloId();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT SUM(Quantità) FROM ProdottiCarrello WHERE CartID = @CartID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CartID", cartId);
                var result = command.ExecuteScalar();
                return result != DBNull.Value ? Convert.ToInt32(result) : 0;
            }
        }
    }
}
