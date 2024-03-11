using MagicVilla_VillaAPI.Models.DTO;

namespace MagicVilla_VillaAPI.Data
{
	public static class VillaStore
	{
		public static List<VillaDTO> VillaList = new List<VillaDTO> {
				new VillaDTO { Id = 1, Name = "beach", Sqft = 10 , Occupancy = 10},
				new VillaDTO { Id = 2, Name = "Honey" ,Sqft = 55 , Occupancy = 1250},
			new VillaDTO { Id = 3, Name = "maxVil",Sqft = 10 , Occupancy = 222 },
				new VillaDTO { Id = 4, Name = "Beauty",Sqft = 1660 , Occupancy = 10 }

		};
	}
}
