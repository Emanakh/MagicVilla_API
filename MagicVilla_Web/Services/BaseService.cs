using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Services.IServices;
using Newtonsoft.Json;
using System.Text;

namespace MagicVilla_Web.Services
{
	public class BaseService : IBaseService
	{
		private readonly IHttpClientFactory _httpClient;

		public APIResponse responseModel { get; set; }

		public BaseService(IHttpClientFactory httpClient)
		{
			responseModel = new();
			_httpClient = httpClient;
		}
		

		public async Task<T> SendAsync<T>(APIRequest apiRequest)
		{
			try
			{
				var client = _httpClient.CreateClient("MagicAPI"); // create client
				HttpRequestMessage message = new HttpRequestMessage();
				message.Headers.Add("Accept", "application/json");
				//convert the url string to the Uri ,, Uri class is immutable -> can't change after setting
				message.RequestUri = new Uri(apiRequest.Url);

				if (apiRequest.Data != null) //as in post /put requests
				{
					message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data), Encoding.UTF8, "application/json");
				}
				switch (apiRequest.apiType)
				{
					case SD.ApiType.POST:
						message.Method = HttpMethod.Post;
						break;

					case SD.ApiType.DELETE:
						message.Method = HttpMethod.Delete;
						break;
					case SD.ApiType.PUT:
						message.Method = HttpMethod.Post;
						break;
					default:
						message.Method = HttpMethod.Get;
						break;

				}
				HttpResponseMessage apiResponse = null; //default
				apiResponse = await client.SendAsync(message);
				//convert the HTTP content to a string
				var apiContent = await apiResponse.Content.ReadAsStringAsync();
				//conert to generic T object
				var APIResponse = JsonConvert.DeserializeObject<T>(apiContent);
				return APIResponse;
			}
			catch (Exception e)
			{
				var dto = new APIResponse
				{
					ErrorMessages = new List<string> { Convert.ToString(e.Message) },
					IsSuccess = false
				};
				//serializing the APIResponse object to JSON and then deserializing it back into a generic type T
								var res = JsonConvert.SerializeObject(dto);
				var APIResponse = JsonConvert.DeserializeObject<T>(res);
				return APIResponse;
			}
		}
	}
}
