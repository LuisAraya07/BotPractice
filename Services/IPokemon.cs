using CedulaBot.Model;
using System.Threading.Tasks;

namespace CedulaBot.Services
{
    public interface IPokemon
    {
        Task<PokemonResponse> consultarPokemon(string pokemon);
    }
}