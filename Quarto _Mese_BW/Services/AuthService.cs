namespace Quarto__Mese_BW.Services
{
    public class AuthService : IAuthService
    {
        public bool Authenticate(string email, string password)
        {
            // Qui verifichiamo le credenziali, in un'applicazione reale queste verrebbero verificate in un database
            return email == "admin@admin.com" && password == "password";
        }

        public bool IsUserLoggedIn()
        {
            // Implementa la logica per verificare se l'utente è loggato
            return true;
        }
    }
}
