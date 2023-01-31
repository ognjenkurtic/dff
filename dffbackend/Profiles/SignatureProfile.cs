using AutoMapper;
using dffbackend.BusinessLogic.Signatures.DTOs;
using dffbackend.Models;

namespace dffbackend.Profiles;

public class SignatureProfile : Profile
{
    public SignatureProfile()
    {
        // Explicit mapping in case of property renaming
        CreateMap<SignatureSetDto, Signature>()
            .ForMember(dest => dest.Signature1, opt => opt.MapFrom(src => src.Signature1))
            .ForMember(dest => dest.Signature2, opt => opt.MapFrom(src => src.Signature2))
            .ForMember(dest => dest.Signature3, opt => opt.MapFrom(src => src.Signature3))
            .ForMember(dest => dest.Signature4, opt => opt.MapFrom(src => src.Signature4))
            .ForMember(dest => dest.Signature5, opt => opt.MapFrom(src => src.Signature5));
    }
}