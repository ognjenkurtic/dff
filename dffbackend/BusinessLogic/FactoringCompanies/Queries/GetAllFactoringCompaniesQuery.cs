using AutoMapper;
using dffbackend.BusinessLogic.FactoringCompanies.DTOs;
using dffbackend.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace dffbackend.BusinessLogic.FactoringCompanies.Queries
{
    public class GetAllFactoringCompaniesQuery: IRequest<List<FactoringCompanyDto>>
    {
    }

    public class GetAllFactoringCompaniesQueryHandler : IRequestHandler<GetAllFactoringCompaniesQuery, List<FactoringCompanyDto>>
    {
        private readonly DffContext _dbContext;
        private readonly IMapper _mapper;

        public GetAllFactoringCompaniesQueryHandler(DffContext context, IMapper mapper)
        {
            _dbContext = context;
            _mapper = mapper;
        }

        public async Task<List<FactoringCompanyDto>> Handle(GetAllFactoringCompaniesQuery request, CancellationToken cancellationToken)
        {
            var factors = await _dbContext.FactoringCompanies
                            .ToListAsync(cancellationToken);

            return this._mapper.Map<List<FactoringCompanyDto>>(factors);
        }
    }
}