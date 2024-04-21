using AutoMapper;
using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;

namespace MagicVilla_Web.Controllers
{
	public class VillaController : Controller
	{
		private readonly IVillaService _villaService;
		private readonly IMapper _mapper;

		public VillaController(IVillaService villaService , IMapper mapper)
        {
			_villaService = villaService;
			_mapper = mapper;
		}
        public IActionResult Index()
		{
			return View();
		}

		public async Task<IActionResult> IndexVilla()
		{
			List<VillaDTO> list = new();
			//the method return T type so i have to specifiy that return type is ApiResponse
			var response = await _villaService.GetAllAsync<APIResponse>();
			if(response != null && response
				.IsSuccess )
			{
				//json to object
				list = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result));
			}
			return View(list);
		}

		public async Task<IActionResult> CreateVilla()
		{
			
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken] 
		// token is unique for each user session and must be submitted along with the form data in subsequent POST requests. -- security
		public async Task<IActionResult> CreateVilla(VillaCreatedDTO model)
		{
			if (ModelState.IsValid)
			{

				var response = await _villaService.CreateAsync<APIResponse>(model);
				if (response != null && response.IsSuccess)
				{
					
					return RedirectToAction(nameof(IndexVilla));
				}
			}
			return View(model);
		}


		
		public async Task<IActionResult> UpdateVilla(int villaId)
		{
			var response = await _villaService.GetAsync<APIResponse>(villaId);
			if (response != null && response.IsSuccess)
			{

				VillaDTO model = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(response.Result));
				return View(_mapper.Map<VillaUpdatedDTO>(model));
			}
			return NotFound();
		}

		[HttpPost]
		public async Task<IActionResult> UpdateVilla(VillaUpdatedDTO model)
		{
			if (ModelState.IsValid)
			{

				var response = await _villaService.UpdateAsync<APIResponse>(model);
				if (response != null && response.IsSuccess)
				{

					return RedirectToAction(nameof(IndexVilla));
				}
			}
			return View(model);

		}


        public async Task<IActionResult> DeleteVilla(int villaId)
        {
            var response = await _villaService.GetAsync<APIResponse>(villaId);
            if (response != null && response.IsSuccess)
            {
                VillaDTO model = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(response.Result));
                return View(model);
            }
            return NotFound();
        }


        [HttpPost]
        public async Task<IActionResult> DeleteVilla(VillaDTO model)
        {

            var response = await _villaService.DeleteAsync<APIResponse>(model.Id);
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(IndexVilla));
            }
            return View(model);
        }

    }
}
