using CedulaBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CedulaBot.Services
{
    public class Gometa : IGometa
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public Gometa(IHttpClientFactory httpClientFactory)
        {
            this._httpClientFactory = httpClientFactory;
        }
        public async Task<Root> consultarCedula(int cedula)
        {
            try
            {
                string apiUrl = $"https://apis.gometa.org/cedulas/{cedula}";
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var responseStream = await response.Content.ReadAsStringAsync();
                    var resultado = JsonSerializer.Deserialize<Root>(responseStream, new JsonSerializerOptions
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
