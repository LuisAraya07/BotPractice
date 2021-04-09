using CedulaBot.Model;
using System.Threading.Tasks;

namespace CedulaBot.Services
{
    public interface IGometa
    {
        Task<Root> consultarCedula(int cedula);
    }
}