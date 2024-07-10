using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Quarto__Mese_BW.Models;
using Quarto__Mese_BW.ViewModels;

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

        private int GetOrCreateCarrelloId()
        {
            var cartIdSession = _httpContextAccessor.HttpContext.Session.GetInt32(SessionKeyCartId);
            if (cartIdSession.HasValue)
            {
                return cartIdSession.Value;
            }

            int cartId;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "INSERT INTO Carrello (DataCreazione) OUTPUT INSERTED.CartID VALUES (@DataCreazione)";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@DataCreazione", DateTime.Now);
                cartId = (int)command.ExecuteScalar();
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

        public Anagrafica GetFirstAnagrafica()
        {
            Anagrafica user = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT TOP 1 * FROM Anagrafica";
                var command = new SqlCommand(query, connection);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new Anagrafica
                        {
                            UserID = reader.GetInt32(0),
                            Nome = reader.GetString(1),
                            Cognome = reader.GetString(2),
                            Email = reader.GetString(3),
                            Via = reader.GetString(4),
                            CAP = reader.GetString(5),
                            Città = reader.GetString(6),
                            Provincia = reader.GetString(7),
                            Telefono = reader.GetString(8)
                        };
                    }
                }
            }
            return user;
        }

        public decimal GetTotaleCarrello()
        {
            var cartId = GetOrCreateCarrelloId();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT SUM(p.Prezzo * pc.Quantità) FROM ProdottiCarrello pc JOIN Prodotti p ON pc.ProductID = p.ProductID WHERE pc.CartID = @CartID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CartID", cartId);
                var result = command.ExecuteScalar();
                return result != DBNull.Value ? Convert.ToDecimal(result) : 0m;
            }
        }

        public void CompletaAcquisto(Anagrafica anagrafica)
        {
            var cartId = GetOrCreateCarrelloId();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Salva i dati dell'anagrafica e ottieni l'ID dell'utente
                var query = @"
            INSERT INTO Anagrafica (Nome, Cognome, Email, Via, CAP, Città, Provincia, Telefono)
            VALUES (@Nome, @Cognome, @Email, @Via, @CAP, @Città, @Provincia, @Telefono);
            SELECT CAST(scope_identity() AS int)";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Nome", anagrafica.Nome);
                command.Parameters.AddWithValue("@Cognome", anagrafica.Cognome);
                command.Parameters.AddWithValue("@Email", anagrafica.Email);
                command.Parameters.AddWithValue("@Via", anagrafica.Via);
                command.Parameters.AddWithValue("@CAP", anagrafica.CAP);
                command.Parameters.AddWithValue("@Città", anagrafica.Città);
                command.Parameters.AddWithValue("@Provincia", anagrafica.Provincia);
                command.Parameters.AddWithValue("@Telefono", anagrafica.Telefono);
                var userId = (int)command.ExecuteScalar();

                // Salva l'ordine e ottieni l'ID dell'ordine
                query = "INSERT INTO Ordini (UserID, DataOrdine, Stato, Totale) OUTPUT INSERTED.OrderID VALUES (@UserID, @DataOrdine, @Stato, @Totale)";
                command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserID", userId);
                command.Parameters.AddWithValue("@DataOrdine", DateTime.Now);
                command.Parameters.AddWithValue("@Stato", "In Elaborazione");
                command.Parameters.AddWithValue("@Totale", GetTotaleCarrello());
                var orderId = (int)command.ExecuteScalar();

                // Salva i prodotti dell'ordine
                var prodottiCarrello = GetCarrelloProdotti();
                foreach (var item in prodottiCarrello)
                {
                    query = "INSERT INTO ProdottiOrdine (OrderID, ProductID, Quantità, PrezzoUnitario) VALUES (@OrderID, @ProductID, @Quantità, @PrezzoUnitario)";
                    command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@OrderID", orderId);
                    command.Parameters.AddWithValue("@ProductID", item.Prodotto.ProductID);
                    command.Parameters.AddWithValue("@Quantità", item.Quantità);
                    command.Parameters.AddWithValue("@PrezzoUnitario", item.Prodotto.Prezzo);
                    command.ExecuteNonQuery();
                }

                // Svuota il carrello
                SvuotaCarrello();
            }
        }

        public OrdineDettaglioViewModel GetOrdineDettaglioById(int orderId)
        {
            OrdineDettaglioViewModel ordineDettaglio = new OrdineDettaglioViewModel();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Recupera i dettagli dell'ordine
                var query = "SELECT o.OrderID, o.UserID, o.DataOrdine, o.Stato, o.Totale, a.Nome, a.Cognome, a.Email, a.Via, a.CAP, a.Città, a.Provincia, a.Telefono FROM Ordini o JOIN Anagrafica a ON o.UserID = a.UserID WHERE o.OrderID = @OrderID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@OrderID", orderId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        ordineDettaglio.Ordine = new Ordine
                        {
                            OrderID = reader.GetInt32(0),
                            UserID = reader.GetInt32(1),
                            DataOrdine = reader.GetDateTime(2),
                            Stato = reader.GetString(3),
                            Totale = reader.GetDecimal(4)
                        };

                        ordineDettaglio.Anagrafica = new Anagrafica
                        {
                            UserID = reader.GetInt32(1),
                            Nome = reader.GetString(5),
                            Cognome = reader.GetString(6),
                            Email = reader.GetString(7),
                            Via = reader.GetString(8),
                            CAP = reader.GetString(9),
                            Città = reader.GetString(10),
                            Provincia = reader.GetString(11),
                            Telefono = reader.GetString(12)
                        };
                    }
                }

                // Recupera i prodotti dell'ordine
                query = "SELECT po.ProductID, po.Quantità, po.PrezzoUnitario, p.Nome, p.Descrizione, p.ImmagineUrl FROM ProdottiOrdine po JOIN Prodotti p ON po.ProductID = p.ProductID WHERE po.OrderID = @OrderID";
                command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@OrderID", orderId);

                using (var reader = command.ExecuteReader())
                {
                    var prodotti = new List<ProdottoOrdine>();
                    while (reader.Read())
                    {
                        var prodottoOrdine = new ProdottoOrdine
                        {
                            ProductID = reader.GetInt32(0),
                            Quantità = reader.GetInt32(1),
                            PrezzoUnitario = reader.GetDecimal(2),
                            Nome = reader.GetString(3),
                            Descrizione = reader.GetString(4),
                            ImmagineUrl = reader.GetString(5)
                        };
                        prodotti.Add(prodottoOrdine);
                    }
                    ordineDettaglio.Prodotti = prodotti;
                }
            }

            return ordineDettaglio;
        }

        public void EliminaOrdine(int orderId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Elimina i prodotti dell'ordine
                        var query = "DELETE FROM ProdottiOrdine WHERE OrderID = @OrderID";
                        var command = new SqlCommand(query, connection, transaction);
                        command.Parameters.AddWithValue("@OrderID", orderId);
                        command.ExecuteNonQuery();

                        // Elimina l'ordine
                        query = "DELETE FROM Ordini WHERE OrderID = @OrderID";
                        command = new SqlCommand(query, connection, transaction);
                        command.Parameters.AddWithValue("@OrderID", orderId);
                        command.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }

                // Resetta l'auto-incremento dell'ID dell'ordine (solo se necessario e con cautela)
                var resetQuery = "DBCC CHECKIDENT ('Ordini', RESEED, 0)";
                var resetCommand = new SqlCommand(resetQuery, connection);
                resetCommand.ExecuteNonQuery();
            }
        }


        public IEnumerable<Ordine> GetOrdini()
        {
            var ordini = new List<Ordine>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM Ordini";
                var command = new SqlCommand(query, connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var ordine = new Ordine
                        {
                            OrderID = reader.GetInt32(0),
                            UserID = reader.GetInt32(1),
                            DataOrdine = reader.GetDateTime(2),
                            Stato = reader.GetString(3),
                            Totale = reader.GetDecimal(4)
                        };
                        ordini.Add(ordine);
                    }
                }
            }
            return ordini;
        }

        public void AggiornaQuantitàProdottoOrdine(int orderId, int productId, int quantità)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "UPDATE ProdottiOrdine SET Quantità = @Quantità WHERE OrderID = @OrderID AND ProductID = @ProductID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Quantità", quantità);
                command.Parameters.AddWithValue("@OrderID", orderId);
                command.Parameters.AddWithValue("@ProductID", productId);
                command.ExecuteNonQuery();
            }
        }

        public void EliminaProdottoOrdine(int orderId, int productId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "DELETE FROM ProdottiOrdine WHERE OrderID = @OrderID AND ProductID = @ProductID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@OrderID", orderId);
                command.Parameters.AddWithValue("@ProductID", productId);
                command.ExecuteNonQuery();
            }
        }

        public int SalvaAnagrafica(Anagrafica anagrafica)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = @"
            INSERT INTO Anagrafica (Nome, Cognome, Email, Via, CAP, Città, Provincia, Telefono)
            VALUES (@Nome, @Cognome, @Email, @Via, @CAP, @Città, @Provincia, @Telefono);
            SELECT CAST(scope_identity() AS int)";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Nome", anagrafica.Nome);
                command.Parameters.AddWithValue("@Cognome", anagrafica.Cognome);
                command.Parameters.AddWithValue("@Email", anagrafica.Email);
                command.Parameters.AddWithValue("@Via", anagrafica.Via);
                command.Parameters.AddWithValue("@CAP", anagrafica.CAP);
                command.Parameters.AddWithValue("@Città", anagrafica.Città);
                command.Parameters.AddWithValue("@Provincia", anagrafica.Provincia);
                command.Parameters.AddWithValue("@Telefono", anagrafica.Telefono);
                var userId = (int)command.ExecuteScalar();
                _httpContextAccessor.HttpContext.Session.SetInt32("UserId", userId);
                return userId;
            }
        }
    }
}
