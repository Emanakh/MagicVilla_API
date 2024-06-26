﻿using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;
using System.Net.Http;

namespace MagicVilla_Web.Services
{
	public class VillaService : BaseService, IVillaService
	{
		private readonly IHttpClientFactory _clientFactory;

		private string villaUrl;

		//base service constructor require IHttpClientFactory
		public VillaService(IHttpClientFactory clientFactory, IConfiguration configuration) :base(clientFactory)
		{
			_clientFactory = clientFactory;
			villaUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");
		}
		public Task<T> CreateAsync<T>(VillaCreatedDTO dto)
		{
			return SendAsync<T>(new APIRequest()
			{
				apiType = MagicVilla_Utility.SD.ApiType.POST,
				Data = dto,
				Url = villaUrl + "/api/villaAPI"
			});
					}

		public Task<T> DeleteAsync<T>(int id)
		{
			return SendAsync<T>(new APIRequest()
			{
				apiType = MagicVilla_Utility.SD.ApiType.DELETE,
				Url = villaUrl + "/api/villaAPI/"+id
			});
		}

		public Task<T> GetAllAsync<T>()
		{
			return SendAsync<T>(new APIRequest()
			{
				apiType = MagicVilla_Utility.SD.ApiType.GET,
				Url = villaUrl + "/api/villaAPI"
            });
		}

		public Task<T> GetAsync<T>(int id)
		{
			return SendAsync<T>(new APIRequest()
			{
				apiType = MagicVilla_Utility.SD.ApiType.GET,
				Url = villaUrl + "/api/villaAPI/"+id
			});
		}

		public Task<T> UpdateAsync<T>(VillaUpdatedDTO dto)
		{
			return SendAsync<T>(new APIRequest()
			{
				apiType = MagicVilla_Utility.SD.ApiType.PUT,
				Data = dto,
				Url = villaUrl + "/api/villaAPI/"+dto.Id
			});
		}
	}
}
