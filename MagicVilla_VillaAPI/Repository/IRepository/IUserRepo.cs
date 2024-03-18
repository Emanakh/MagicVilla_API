
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;

namespace MagicVilla_VillaAPI.Repository.IRepository
{
	public interface IUserRepo
	{
		public bool IsUniqueUser(string username);
		Task<LoginResDTO> Login(LoginReqDTO loginReqDTO);
		Task<localUser> Register(RegisterReqDTO registerReqDTO);

	}
}
