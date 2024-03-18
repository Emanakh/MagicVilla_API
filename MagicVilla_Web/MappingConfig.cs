using AutoMapper;
using MagicVilla_Web.Models.DTO;

namespace MagicVilla_Web
{
	public class MappingConfig : Profile
	{
		public MappingConfig()
		{
			CreateMap<VillaDTO, VillaCreatedDTO>().ReverseMap();
			CreateMap<VillaDTO, VillaUpdatedDTO>().ReverseMap();


			CreateMap<VillaNumberDTO, VillaNumberCreateDTO>().ReverseMap();
			CreateMap<VillaNumberDTO, VillaNumberUpdateDTO>().ReverseMap();

		}
	}
}
