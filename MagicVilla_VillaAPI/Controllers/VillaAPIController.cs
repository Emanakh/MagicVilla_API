﻿using MagicVilla_VillaAPI.Data;
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
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult<IEnumerable<VillaDTO>> GetVillas()
		{
			return Ok(VillaStore.VillaList);

		}


		[HttpGet("{id:int}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult<VillaDTO> GetVilla(int id)
		{
			if (id == 0)
			{
				return BadRequest();
			}
			var villa = VillaStore.VillaList.FirstOrDefault(s => s.Id == id);
			if (villa == null)
			{
				return NotFound();
			}
			return Ok(villa);


		}
	}
}
