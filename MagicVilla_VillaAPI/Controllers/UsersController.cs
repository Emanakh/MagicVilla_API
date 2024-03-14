using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
	[Route("api/UsersAuth")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IUserRepo _userRepo;

		public UsersController(IUserRepo userRepo)
		{
			_userRepo = userRepo;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginReqDTO model)
		{
			var LoginRes = await _userRepo.Login(model);
			if (LoginRes.User == null || string.IsNullOrEmpty(LoginRes.Token))
			{
				//return badrequest(model) i don't get why it made a new messsage
				return BadRequest(new { message = "user or pass are in correct" });
				//هنعمل standard apirespone okay feh status 400, fail, errmsg is the msg elly fo2 okay .. and i will sent it in the bad request 
			}

			//هنعمل standard apirespone okay feh status 200, success, resuk=lt is the login res elly fo2 okay .. and i will sent it in the ok request  
			return Ok(LoginRes);
		}

		[HttpPost("Register")]
		public async Task<IActionResult> Register([FromBody] RegisterReqDTO model)
		{
			bool isUserNameUnique = _userRepo.IsUniqueUser(model.UserName);
			if (!isUserNameUnique)
			{
				//هنعمل standard apirespone okay feh status 400, fail, errmsg is the msg in the bad request 
				return BadRequest();
			}
			var user = await _userRepo.Register(model);
			if (user == null)
			{
				return BadRequest(); //standard response 
			}
			return Ok();
		}






	}
}
