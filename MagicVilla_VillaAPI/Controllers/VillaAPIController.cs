using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
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
		private readonly ApplicationDbContext _db;
		private readonly IMapper _mapper;

		public VillaAPIController(ILogger<VillaAPIController> logger, ApplicationDbContext db, IMapper mapper)
		{
			_logger = logger;
			_db = db;
			_mapper = mapper;
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
		{
			_logger.LogInformation("getting all villas");

			IEnumerable<Villa> VillaList = await _db.Villas.ToListAsync();
			return Ok(_mapper.Map<List<VillaDTO>>(VillaList));

		}


		[HttpGet("{id:int}", Name = "GetVilla")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<VillaDTO>> GetVilla(int id)
		{
			if (id == 0)
			{
				return BadRequest();
			}
			//var villa = _db.Villas.FirstOrDefault(s => s.Id == id);
			var villa = await _db.Villas.FindAsync(id);
			if (villa == null)
			{
				_logger.LogError($"can't find villa with id = {id}");
				return NotFound();
			}
			return Ok(_mapper.Map<VillaDTO>(villa));
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody] VillaCreatedDTO createDTO)
		{
			//if (!ModelState.IsValid) { return BadRequest(ModelState); } //api controller will handle the validations by the data annotations -- the breakpoint will not even enter the model state validations 

			//--- i can add custom validations by model state
			if (await _db.Villas.FirstOrDefaultAsync(s => s.Name == createDTO.Name) != null)
			{
				ModelState.AddModelError("custom validation", "the name should be unique");
				return BadRequest(ModelState);
			}

			if (createDTO == null)
			{
				return BadRequest(createDTO);
			}

			Villa model = _mapper.Map<Villa>(createDTO);
			model.CreatedDate = DateTime.Now;

			await _db.Villas.AddAsync(model);
			await _db.SaveChangesAsync();
			return CreatedAtRoute("GetVilla", new { id = model.Id }, model); //the new identity id from the db villa..
		}

		[HttpDelete("{id:int}", Name = "DeleteVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> DeleteVilla(int id) //Iaction result instead of actionresult cuz there's no return type needed..
		{
			if (id == 0)
			{
				return BadRequest();
			}
			var villa = await _db.Villas.FindAsync(id);
			//var villa = _db.Villas.FirstOrDefault(s => s.Id == id);
			if (villa == null)
			{
				return NotFound();
			}
			_db.Villas.Remove(villa);
			await _db.SaveChangesAsync();
			return NoContent();

		}




		[HttpPut("{id:int}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdatedDTO updateDTO)
		{
			if (updateDTO == null || updateDTO.Id != id)
			{
				return BadRequest();
			}
			var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
			if (villa == null) //added this by my own..
			{
				return NotFound();
			}
			//i feel that i need to check the new name..

			Villa model = _mapper.Map<Villa>(updateDTO);
			model.UpdatedDate = DateTime.Now;

			_db.Villas.Update(model);
			await _db.SaveChangesAsync();
			return NoContent();
		}


		[HttpPatch("{id:int}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdatedDTO> patchVilla)
		{
			if (patchVilla == null || id == 0)
			{
				return BadRequest();
			}
			var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
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

			_db.Villas.Update(model);
			await _db.SaveChangesAsync();
			return NoContent();
		}




	}
}
