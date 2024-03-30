using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Controllers
{

	[ApiController]
	//[Route("api/[Controller]")]
	[Route("api/VillaAPI")]
	public class VillaAPIController : ControllerBase
	{
		private readonly ILogger<VillaAPIController> _logger;
		private readonly IVillaRepository _dbVilla;
		private readonly IMapper _mapper;
		private APIResponse _apiResponse;

		public VillaAPIController(ILogger<VillaAPIController> logger, IVillaRepository dbVilla, IMapper mapper)
		{
			_logger = logger;
			_dbVilla = dbVilla;
			_mapper = mapper;
			_apiResponse = new APIResponse();
		}



		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<APIResponse>> GetVillas()
		{
			try
			{
				_logger.LogInformation("getting all villas");

				IEnumerable<Villa> VillaList = await _dbVilla.GetAllAsync();
				_apiResponse.Result = _mapper.Map<List<VillaDTO>>(VillaList);
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



		[HttpGet("{id:int}", Name = "GetVilla")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<APIResponse>> GetVilla(int id)
		{
			try
			{
				if (id == 0)
				{
					return BadRequest();
				}
				//var villa = _db.Villas.FirstOrDefault(s => s.Id == id);
				var villa = await _dbVilla.GetAsync(v => v.Id == id);
				if (villa == null)
				{
					_logger.LogError($"can't find villa with id = {id}");
					return NotFound();
				}
				_apiResponse.Result = _mapper.Map<VillaDTO>(villa);
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
		public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaCreatedDTO createDTO)
		{
			//if (!ModelState.IsValid) { return BadRequest(ModelState); } //api controller will handle the validations by the data annotations -- the breakpoint will not even enter the model state validations 

			//--- i can add custom validations by model state
			try
			{
				if (await _dbVilla.GetAsync(n => n.Name == createDTO.Name) != null)
				{
					ModelState.AddModelError("custom validation", "the name should be unique");
					return BadRequest(ModelState);
				}


				if (createDTO == null)
				{
					return BadRequest(createDTO);
				}

				Villa villa = _mapper.Map<Villa>(createDTO);
				villa.CreatedDate = DateTime.Now;
				await _dbVilla.CreateAsync(villa);

				_apiResponse.Result = _mapper.Map<VillaDTO>(villa);
				_apiResponse.StatusCode = System.Net.HttpStatusCode.Created;
				_apiResponse.IsSuccess = true;

				return CreatedAtRoute("GetVilla", new { id = villa.Id }, _apiResponse); //the new identity id from the db villa..
			}
			catch (Exception ex)
			{
				_apiResponse.IsSuccess = false;
				_apiResponse.ErrorMessages = new List<string> { ex.Message };

			}
			return _apiResponse;
		}



		[HttpDelete("{id:int}", Name = "DeleteVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<APIResponse>> DeleteVilla(int id) //Iaction result instead of actionresult cuz there's no return type needed..
		{
			try
			{
				if (id == 0)
				{
					return BadRequest();
				}
				var villa = await _dbVilla.GetAsync(v => v.Id == id);
				//var villa = _db.Villas.FirstOrDefault(s => s.Id == id);
				if (villa == null)
				{
					return NotFound();
				}
				await _dbVilla.RemoveAsync(villa);
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
		public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody] VillaUpdatedDTO updateDTO)
		{
			try
			{
				if (updateDTO == null || updateDTO.Id != id)
				{
					return BadRequest();
				}
				var villa = await _dbVilla.GetAsync(v => v.Id == id, tracked: false); //or false directly
				if (villa == null) //added this by my own..
				{
					return NotFound();
				}
				//i feel that i need to check the new name..

				Villa model = _mapper.Map<Villa>(updateDTO);
				model.CreatedDate = DateTime.Now;
				await _dbVilla.UpdateAsync(model);

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


		[HttpPatch("{id:int}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<APIResponse>> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdatedDTO> patchVilla)
		{
			try
			{

				if (patchVilla == null || id == 0)
				{
					return BadRequest();
				}
				var villa = await _dbVilla.GetAsync(v => v.Id == id, tracked: false);
				if (villa == null)
				{
					return NotFound();
				}
				//did update in the dto first.. 
				VillaUpdatedDTO villaDTO = _mapper.Map<VillaUpdatedDTO>(villa);

				patchVilla.ApplyTo(villaDTO, ModelState); //add model state to log if there's errors.

				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				//convert dto to villa type
				Villa model = _mapper.Map<Villa>(villaDTO);
				model.CreatedDate = DateTime.Now;
				await _dbVilla.UpdateAsync(model);

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
