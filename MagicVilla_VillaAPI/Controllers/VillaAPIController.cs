using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{

	[ApiController]
	//[Route("api/[Controller]")]
	[Route("api/VillaAPI")]
	public class VillaAPIController : ControllerBase
	{
		[HttpGet]
		public IEnumerable<VillaDTO> GetVillas()
		{
			return new List<VillaDTO> {
				new VillaDTO { Id = 1, Name = "beach" },
				new VillaDTO { Id = 2, Name = "Honey" }
			};
		}
	}
}
