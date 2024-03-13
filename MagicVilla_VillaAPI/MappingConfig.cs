using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;

namespace MagicVilla_VillaAPI
{
	public class MappingConfig : Profile
	{
		public MappingConfig()
		{
			CreateMap<Villa, VillaDTO>().ReverseMap();
			CreateMap<Villa, VillaCreatedDTO>().ReverseMap();
			CreateMap<Villa, VillaUpdatedDTO>().ReverseMap();

			CreateMap<VillaNumber, VillaNumberDTO>().ReverseMap();
			CreateMap<VillaNumber, VillaNumberCreateDTO>().ReverseMap();
			CreateMap<VillaNumber, VillaNumberUpdateDTO>().ReverseMap();

		}
	}
}
