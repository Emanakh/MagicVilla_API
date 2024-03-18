using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using MagicVilla_VillaAPI.Repository.Repository;
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
		public UserRepo(ApplicationDbContext db, IConfiguration configuration)
		{
			_db = db;
			secretKey = configuration.GetValue<string>("ApiSettings:secret");
		}
		public bool IsUniqueUser(string username)
		{
			var user = _db.LocalUsers.FirstOrDefault(x => x.UserName == username);
			if (user == null)
			{
				return true;
			}
			return false;
		}

		//generate token
		public async Task<LoginResDTO> Login(LoginReqDTO loginReqDTO)
		{
			var user = _db.LocalUsers.FirstOrDefault(u => u.UserName.ToLower() == loginReqDTO.UserName.ToLower() && u.Password == loginReqDTO.Password);
			if (user == null)
			{
				//return null;
				return new LoginResDTO()
				{
					Token = "",
					User = null

				};
			}
			//if found generate token... 

			//key 
			var secretKeyInBytes = Encoding.ASCII.GetBytes(secretKey);
			var key = new SymmetricSecurityKey(secretKeyInBytes);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
			{
				new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
				new Claim(ClaimTypes.Role, user.Role),
			}),
				Expires = DateTime.UtcNow.AddDays(7),
				SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
			};

			var TokenHandler = new JwtSecurityTokenHandler();
			var token = TokenHandler.CreateToken(tokenDescriptor);
			var StringToken = TokenHandler.WriteToken(token);
			LoginResDTO loginResDTO = new LoginResDTO()
			{
				Token = StringToken,
				User = new UserDTO
				{
					ID = user.ID.ToString(),
					Name = user.Name,
					UserName = user.UserName
				}
			};
			return loginResDTO;

		}

		public async Task<localUser> Register(RegisterReqDTO registerReqDTO)
		{
			localUser user = new localUser()
			{
				UserName = registerReqDTO.UserName,
				Password = registerReqDTO.Password,
				Role = registerReqDTO.Role,
				Name = registerReqDTO.Name

			};
			await _db.LocalUsers.AddAsync(user); //?? he didn't await !!! 
			await _db.SaveChangesAsync();
			user.Password = "";
			return user;

		}
	}
}