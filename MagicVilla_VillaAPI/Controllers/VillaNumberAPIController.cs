using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MagicVilla_VillaAPI.Controllers
{

	[ApiController]
	//[Route("api/[Controller]")]
	[Route("api/VillaNumberAPI")]
	public class VillaNumberAPIController : ControllerBase
	{
		private readonly IVillaNumberRepository _dbVillaNumber;
		private readonly IVillaRepository _dbvilla;
		private readonly IMapper _mapper;
		private APIResponse _apiResponse;

		public VillaNumberAPIController(IVillaNumberRepository dbVillaNumber, IVillaRepository dbvilla, IMapper mapper)
		{
			_dbVillaNumber = dbVillaNumber;
			_dbvilla = dbvilla;
			_mapper = mapper;
			_apiResponse = new APIResponse();
		}



		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<APIResponse>> GetVillaNumbers()
		{
			try
			{

				IEnumerable<VillaNumber> VillaNumberList = await _dbVillaNumber.GetAllAsync();
				_apiResponse.Result = _mapper.Map<List<VillaNumberDTO>>(VillaNumberList);
				_apiResponse.IsSuccess = true;
				_apiResponse.StatusCode = System.Net.HttpStatusCode.OK;
				return Ok(_apiResponse);
			}
			catch (Exception ex)
			{
				_apiResponse.IsSuccess = false;
				_apiResponse.ErrorMessages = new List<string> { ex.Message };

			}
			return _apiResponse;

		}



		[HttpGet("{id:int}", Name = "GetVillaNumber")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<APIResponse>> GetVillaNumber(int id)
		{
			try
			{
				if (id == 0)
				{
					return BadRequest();
				}
				var villaNumber = await _dbVillaNumber.GetAsync(v => v.VillaNo == id);
				if (villaNumber == null)
				{
					return NotFound();
				}
				_apiResponse.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
				_apiResponse.StatusCode = System.Net.HttpStatusCode.OK;
				_apiResponse.IsSuccess = true;
				return Ok(_apiResponse);
			}
			catch (Exception ex)
			{
				_apiResponse.IsSuccess = false;
				_apiResponse.ErrorMessages = new List<string> { ex.Message };

			}
			return _apiResponse;

		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDTO createDTO)
		{
			try
			{
				if (await _dbVillaNumber.GetAsync(n => n.VillaNo == createDTO.VillaNo) != null)
				{
					ModelState.AddModelError("custom validation", "Villa Number already Existed");
					return BadRequest(ModelState);
				}

				if (await _dbvilla.GetAsync(v => v.Id == createDTO.VillaID) == null)
				{
					ModelState.AddModelError("custom validation", "Villa ID don't Existed");
					return BadRequest(ModelState);
				}

				if (createDTO == null)
				{
					return BadRequest(createDTO);
				}

				VillaNumber villaNumber = _mapper.Map<VillaNumber>(createDTO);
				villaNumber.CreatedDate = DateTime.Now;
				await _dbVillaNumber.CreateAsync(villaNumber);

				_apiResponse.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
				_apiResponse.StatusCode = System.Net.HttpStatusCode.Created;
				_apiResponse.IsSuccess = true;

				return CreatedAtRoute("GetVillaNumber", new { id = villaNumber.VillaNo }, _apiResponse);
			}
			catch (Exception ex)
			{
				_apiResponse.IsSuccess = false;
				_apiResponse.ErrorMessages = new List<string> { ex.Message };

			}
			return _apiResponse;
		}



		[HttpDelete("{id:int}", Name = "DeleteVillaNumber")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int id)
		{
			try
			{
				if (id == 0)
				{
					return BadRequest();
				}
				var villa = await _dbVillaNumber.GetAsync(v => v.VillaNo == id);
				if (villa == null)
				{
					return NotFound();
				}
				await _dbVillaNumber.RemoveAsync(villa);
				_apiResponse.StatusCode = System.Net.HttpStatusCode.NoContent;
				_apiResponse.IsSuccess = true;

				return Ok(_apiResponse);
			}
			catch (Exception ex)
			{
				_apiResponse.IsSuccess = false;
				_apiResponse.ErrorMessages = new List<string> { ex.Message };

			}
			return _apiResponse;

		}




		[HttpPut("{id:int}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int id, [FromBody] VillaNumberUpdateDTO updateDTO)
		{
			try
			{
				if (updateDTO == null || updateDTO.VillaNo != id)
				{
					return BadRequest();
				}

				if (await _dbvilla.GetAsync(v => v.Id == updateDTO.VillaID) == null)
				{
					ModelState.AddModelError("custom validation", "Villa ID don't Existed");
					return BadRequest(ModelState);
				}



				var villaNumber = await _dbVillaNumber.GetAsync(v => v.VillaNo == id, tracked: false); //or false directly
				if (villaNumber == null) //added this by my own..
				{
					return NotFound();
				}
				//i feel that i need to check the new name..

				VillaNumber model = _mapper.Map<VillaNumber>(updateDTO);
				model.CreatedDate = DateTime.Now;
				await _dbVillaNumber.UpdateAsync(model);

				_apiResponse.StatusCode = System.Net.HttpStatusCode.NoContent;
				_apiResponse.IsSuccess = true;

				return Ok(_apiResponse);
			}
			catch (Exception ex)
			{
				_apiResponse.IsSuccess = false;
				_apiResponse.ErrorMessages = new List<string> { ex.Message };

			}
			return _apiResponse;

		}






	}
}
