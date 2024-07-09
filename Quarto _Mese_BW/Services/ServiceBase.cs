using System.Data.Common;

namespace Quarto__Mese_BW.Services
{
    public abstract class ServiceBase
    {
        protected abstract DbConnection GetConnection();
        protected abstract DbCommand GetCommand(string commandText);
    }
}
