using AutoMapper;
using dffbackend.DTOs;
using dffbackend.Models;

namespace dffbackend.Profiles;

public class FactoringCompanyProfile : Profile
{
    public FactoringCompanyProfile()
    {
        CreateMap<CreateFactoringCompanyDto, FactoringCompany>();
    }
}