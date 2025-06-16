using AutoMapper;
using static System.Runtime.InteropServices.JavaScript.JSType;
using VitrineApi.DTOs;
using VitrineApi.Models;

namespace VitrineApi.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Produto, ProdutoDTO>();
        }
    }
}