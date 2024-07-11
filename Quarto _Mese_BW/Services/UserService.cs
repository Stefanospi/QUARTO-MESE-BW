using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Quarto__Mese_BW.Models;
using Microsoft.Extensions.Logging;

namespace Quarto__Mese_BW.Services
{
    public class UserService
    {
        private readonly string _connectionString;
        private readonly ILogger<UserService> _logger;

        public UserService(IConfiguration configuration, ILogger<UserService> logger)
        {
            _connectionString = configuration.GetConnectionString("ECommerce");
            _logger = logger;
        }

        public Anagrafica GetUserById(int userId)
        {
            Anagrafica user = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM Anagrafica WHERE UserID = @UserID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserID", userId);

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

                        _logger.LogInformation($"Utente trovato: {user.Nome} {user.Cognome}");
                    }
                    else
                    {
                        _logger.LogWarning("Utente non trovato");
                    }
                }
            }

            return user;
        }
    }
}
