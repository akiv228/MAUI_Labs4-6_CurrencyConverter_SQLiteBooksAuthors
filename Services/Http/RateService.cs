using Lab1.Entities.Http;
using Microsoft.Maui.Networking;
using Newtonsoft.Json;
using System.Net.Http;

namespace Lab1.Services.Http
{
    public class RateService : IRateService
    {
        private readonly HttpClient _httpClient;
        private readonly IConnectivity _connectivity;

        public RateService(IHttpClientFactory httpClientFactory, IConnectivity connectivity)
        {
            _httpClient = httpClientFactory.CreateClient("NB RB");
            _connectivity = connectivity;
        }

        public async Task<IEnumerable<Rate>> GetRates(DateTime date)
        {
            var result = await GetRatesWithStatus(date);
            return result.Success ? result.Rates : Enumerable.Empty<Rate>();
        }

        public async Task<(bool Success, IEnumerable<Rate> Rates, string ErrorMessage)> GetRatesWithStatus(DateTime date)
        {
            
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                return (false, Enumerable.Empty<Rate>(), "Нет подключения к интернету. Проверьте Wi-Fi или мобильные данные.");
            }

            try
            {
                var formattedDate = date.ToString("yyyy-MM-dd");
                var response = await _httpClient.GetAsync($"?ondate={formattedDate}&periodicity=0");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var rates = JsonConvert.DeserializeObject<IEnumerable<Rate>>(content) 
                                ?? Enumerable.Empty<Rate>();

                    return (true, rates, string.Empty);
                }
                else
                {
                    return (false, Enumerable.Empty<Rate>(), $"Ошибка сервера: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                return (false, Enumerable.Empty<Rate>(), $"Ошибка сети: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, Enumerable.Empty<Rate>(), $"Неизвестная ошибка: {ex.Message}");
            }
        }
    }
}