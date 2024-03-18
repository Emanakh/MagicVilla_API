using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVilla_VillaAPI.Repository.Repository
{
	public class UserRepo : IUserRepo
	{
		private readonly ApplicationDbContext _db;
		private readonly string secretKey;
		private readonly UserManager<ApplicationUser> _userManager;
		public UserRepo(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IConfiguration configuration)
		{
			_userManager = userManager;
			_db = db;
			secretKey = configuration.GetValue<string>("ApiSettings:secret");
		}

		public bool IsUniqueUser(string username)
		{
			var user = _db.ApplicationUsers.FirstOrDefault(x => x.UserName == username);
			if (user == null)
			{
				return true;
			}
			return false;
		}

		public async Task<LoginResDTO> Login(LoginReqDTO loginRequestDTO)
		{
			var user = _db.ApplicationUsers
				.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower());

			bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);


			if (user == null || isValid == false)
			{
				return new LoginResDTO()
				{
					Token = "",
					User = null
				};
			}

			//if user was found generate JWT Token
			var roles = await _userManager.GetRolesAsync(user);
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(secretKey);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
					new Claim(ClaimTypes.Name, user.UserName.ToString()),
					new Claim(ClaimTypes.Role, roles.FirstOrDefault())
				}),
				Expires = DateTime.UtcNow.AddDays(7),
				SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			LoginResDTO loginResponseDTO = new LoginResDTO()
			{
				Token = tokenHandler.WriteToken(token),
				User = new UserDTO
				{
					Name = user.UserName,
					ID = user.Id,
					UserName = user.UserName
				}

			};
			return loginResponseDTO;
		}

		public async Task<UserDTO> Register(RegisterReqDTO registerationRequestDTO)
		{
			ApplicationUser user = new()
			{
				UserName = registerationRequestDTO.UserName,
				Email = registerationRequestDTO.UserName,
				NormalizedEmail = registerationRequestDTO.UserName.ToUpper(),
				Name = registerationRequestDTO.Name
			};

			try
			{
				var result = await _userManager.CreateAsync(user, registerationRequestDTO.Password);
				if (result.Succeeded)
				{
					//if (!_roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult())
					//{
					//	await _roleManager.CreateAsync(new IdentityRole("admin"));
					//	await _roleManager.CreateAsync(new IdentityRole("customer"));
					//}
					await _userManager.AddToRoleAsync(user, "admin");
					var userToReturn = _db.ApplicationUsers
						.FirstOrDefault(u => u.UserName == registerationRequestDTO.UserName);
					return new UserDTO
					{
						Name = userToReturn.Name,
						UserName = userToReturn.UserName,
						ID = userToReturn.Id
					};

				}
			}
			catch (Exception e)
			{

			}

			return new UserDTO();
		}



		//public bool IsUniqueUser(string username)
		//{
		//	var user = _db.ApplicationUsers.FirstOrDefault(x => x.UserName == username);
		//	if (user == null)
		//	{
		//		return true;
		//	}
		//	return false;
		//}

		//generate token
		//public async Task<LoginResDTO> Login(LoginReqDTO loginReqDTO)
		//{
		//	var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginReqDTO.UserName.ToLower());
		//	bool isValid = await _userManager.CheckPasswordAsync(user, loginReqDTO.Password);



		//	if (user == null || isValid == false)
		//	{
		//		//return null;
		//		return new LoginResDTO()
		//		{
		//			Token = "",
		//			User = null

		//		};
		//	}

		//	//if found
		//	var roles = await _userManager.GetRolesAsync(user);
		//	var Role = roles.FirstOrDefault();

		//	//if found generate token... 

		//	//key 
		//	var secretKeyInBytes = Encoding.ASCII.GetBytes(secretKey);
		//	var key = new SymmetricSecurityKey(secretKeyInBytes);

		//	var tokenDescriptor = new SecurityTokenDescriptor
		//	{
		//		Subject = new ClaimsIdentity(new Claim[]
		//	{
		//		new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
		//		new Claim(ClaimTypes.Role, Role),
		//	}),
		//		Expires = DateTime.UtcNow.AddDays(7),
		//		SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
		//	};

		//	var TokenHandler = new JwtSecurityTokenHandler();
		//	var token = TokenHandler.CreateToken(tokenDescriptor);
		//	var StringToken = TokenHandler.WriteToken(token);
		//	LoginResDTO loginResDTO = new LoginResDTO()
		//	{
		//		Token = StringToken,
		//		Role = Role,
		//		User = new UserDTO
		//		{

		//			ID = user.Id,
		//			Name = user.Name,
		//			UserName = user.UserName

		//		}
		//	};
		//	return loginResDTO;

		//}

		//public async Task<UserDTO> Register(RegisterReqDTO registerReqDTO)
		//{
		//	ApplicationUser user = new ApplicationUser()
		//	{

		//		UserName = registerReqDTO.UserName,
		//		Email = registerReqDTO.UserName,
		//		NormalizedEmail = registerReqDTO.UserName.ToUpper(),
		//		Name = registerReqDTO.Name
		//	};
		//	try
		//	{
		//		var result = await _userManager.CreateAsync(user, registerReqDTO.Password);
		//		if (result.Succeeded)
		//		{
		//			await _userManager.AddToRoleAsync(user, "admin");

		//			var newUser = _db.ApplicationUsers.FirstOrDefault(u => u.UserName == registerReqDTO.UserName);
		//			return new UserDTO()
		//			{
		//				ID = newUser.Id,
		//				UserName = newUser.UserName,
		//				Name = newUser.Name
		//			};

		//		}
		//	}
		//	catch (Exception e)
		//	{

		//	}

		//	return new UserDTO();




		//}
	}
}