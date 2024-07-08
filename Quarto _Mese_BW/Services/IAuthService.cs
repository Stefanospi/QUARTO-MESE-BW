namespace Quarto__Mese_BW.Services
{
    public interface IAuthService
    {
        bool Authenticate(string email, string password);
    }
}
