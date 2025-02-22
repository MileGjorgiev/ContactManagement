using ContactManagement.BLL.Abstract;
using ContactManagement.BLL.Requests;
using ContactManagement.DAL.Abstract;
using ContactManagement.Models.Entities;
using MediatR;

namespace ContactManagement.BLL.Handlers
{
    public class GetAllCountriesHandler : IRequestHandler<GetAllCountriesQuery, List<Country>>
    {
        private readonly ICountryRepository _countryRepository;

        public GetAllCountriesHandler(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public async Task<List<Country>> Handle(GetAllCountriesQuery request, CancellationToken cancellationToken)
        {
            return await _countryRepository.GetAllAsync();
        }
    }
}
