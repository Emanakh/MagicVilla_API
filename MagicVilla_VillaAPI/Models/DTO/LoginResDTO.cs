namespace MagicVilla_VillaAPI.Models.DTO
{
	public class LoginResDTO
	{
		public UserDTO User { get; set; }
		public string Token { get; set; }
		public string Role { get; set; }
	}
}
