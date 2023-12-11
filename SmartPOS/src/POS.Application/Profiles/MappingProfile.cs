using AutoMapper;
using System.Data;
using POS.Application.Models.Entities;
using POS.Domain.Entities;

namespace POS.Application.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Category, CategoryVM>().ReverseMap();
    }
}