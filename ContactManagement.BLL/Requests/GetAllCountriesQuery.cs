using ContactManagement.Models.Entities;
using MediatR;

namespace ContactManagement.BLL.Requests
{
    public class GetAllCountriesQuery : IRequest<List<Country>>
    {
    }
}
