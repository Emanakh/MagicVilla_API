﻿using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Models.DTO
{
	public class VillaDTO
	{
		public int Id { get; set; }
		[Required]
		[MaxLength(300)]
		public string Name { get; set; }

	}
}
