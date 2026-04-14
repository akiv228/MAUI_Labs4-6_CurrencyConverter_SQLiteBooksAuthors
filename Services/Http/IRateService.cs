using Lab1.Entities.Http;

namespace Lab1.Services.Http
{
    public interface IRateService
    {
        Task<IEnumerable<Rate>> GetRates(DateTime date);
        Task<(bool Success, IEnumerable<Rate> Rates, string ErrorMessage)> GetRatesWithStatus(DateTime date);
    }
}
