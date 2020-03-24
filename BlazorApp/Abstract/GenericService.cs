﻿using BlazorApp.Models;
using Domain.Abstract;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BlazorApp.Abstract
{
    public abstract class GenericService<TEntity> where TEntity : Entity
    {
        readonly HostConfiguration HostConfiguration;
        readonly protected string ApiUrl;

        public GenericService(HostConfiguration hostConfiguration)
        {
            HostConfiguration = hostConfiguration;
            ApiUrl = $"{HostConfiguration.BankAPIUrl}/api/{typeof(TEntity).Name}".ToLowerInvariant();
        }

        public virtual async Task<APIResponse<TEntity>> GetBanksAsync(int skip = 0, int count = 0)
        {
            using var client = new HttpClient();
            var response = client.GetAsync(ApiUrl).Result;

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<APIResponse<TEntity>>(jsonString);
            }

            return new APIResponse<TEntity>();
        }

        public async Task<bool> DeleteBankAsync(TEntity entity)
        {
            using var client = new HttpClient();
            var response = await client.DeleteAsync($"{ApiUrl}/{entity.Id}");

            return response.IsSuccessStatusCode;
        }

        public virtual async Task<bool> AddBankAsync(TEntity entity)
        {
            var json = JsonConvert.SerializeObject(entity);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var client = new HttpClient();
            var response = await client.PostAsync(ApiUrl, content);
            return response.IsSuccessStatusCode;
        }
    }
}