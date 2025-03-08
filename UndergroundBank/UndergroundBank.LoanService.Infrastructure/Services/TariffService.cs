using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UndergroundBank.Common.Middlewares;
using UndergroundBank.LoanService.Application.Dto.Tariff;
using UndergroundBank.LoanService.Application.Interfaces;
using UndergroundBank.LoanService.Domain.Entities;

namespace UndergroundBank.LoanService.Infrastructure.Services
{
    public class TariffService : ITariffService
    {

        private readonly IMapper _mapper;
        private readonly LoanDbContext _dbContext;

        public TariffService(IMapper mapper, LoanDbContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }
        public async Task CreateTariff(CreateTariffDto createTariffDto)
        {
            var tariff = _mapper.Map<Tariff>(createTariffDto);
            await _dbContext.AddAsync(tariff);
        }

        public Task DeleteTariff(Guid tariffId)
        {
            throw new NotImplementedException();
        }

        public async Task<GetTariffsDto> GetAllTariffs()
        {
            var tariffs = await _dbContext.Tariffs.ToListAsync();
            var tariffsDto = _mapper.Map<List<GetTariffDto>>(tariffs);
            var getTariffsDto = new GetTariffsDto()
            {
                Tariffs = tariffsDto
            };
            return getTariffsDto;
        }

        public async Task<GetTariffDto> GetTariff(Guid tariffId)
        {
            var tariff = await _dbContext.Tariffs.Where(t => t.Id == tariffId).FirstOrDefaultAsync();
            if (tariff == null)
            {
                throw new NotFoundException("Тарифа с таким id не сущетсвует!");
            }
            var tariffDto = _mapper.Map<GetTariffDto>(tariff);
            return tariffDto;
        }

        public Task UpdateTariff()
        {
            throw new NotImplementedException();
        }
    }
}
