using MagicVilla_Web.Models.DTO;

namespace MagicVilla_Web.Services.IServices
{
	public interface IVillaService
	{
		Task<T> GetAllAsync<T>();
		Task<T> GetAsync<T>(int id);
		Task<T> CreateAsync<T>(VillaCreatedDTO dto);
		Task<T> UpdateAsync<T>(VillaUpdatedDTO dto);
		Task<T> DeleteAsync<T>(int id);
	}
}
