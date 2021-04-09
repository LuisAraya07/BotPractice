using CedulaBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CedulaBot.Services
{
    public class Pokemon : IPokemon
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public Pokemon(IHttpClientFactory httpClientFactory)
        {
            this._httpClientFactory = httpClientFactory;
        }
        public async Task<PokemonResponse> consultarPokemon(string pokemon)
        {
            try
            {
                string apiUrl = $"https://pokeapi.co/api/v2/pokemon/{pokemon}";
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var responseStream = await response.Content.ReadAsStringAsync();
                    var resultado = JsonSerializer.Deserialize<PokemonResponse>(responseStream, new JsonSerializerOptions
                    {
                        ReadCommentHandling = JsonCommentHandling.Skip,
                        AllowTrailingCommas = true,
                        PropertyNameCaseInsensitive = true,
                    });
                    
                    return resultado;
                }
                return null;

            }
            catch (Exception)
            {

                return null;
            }
            
        }
    }
}
