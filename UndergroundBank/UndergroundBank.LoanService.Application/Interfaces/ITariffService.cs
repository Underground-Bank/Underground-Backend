using UndergroundBank.LoanService.Application.Dto.Tariff;

namespace UndergroundBank.LoanService.Application.Interfaces
{
    public interface ITariffService
    {
        public Task CreateTariff(CreateTariffDto createTariffDto);
        public Task DeleteTariff(Guid tariffId);
        public Task<GetTariffDto> GetTariff(Guid tariffId);
        public Task<GetTariffsDto> GetAllTariffs();
        public Task UpdateTariff();
    }
}
