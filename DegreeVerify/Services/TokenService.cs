using DegreeVerify.Client.Services.IServices;
using DegreeVerify.DTO;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DegreeVerify.Client.Services
{
    public class TokenService
    {
        private readonly HttpClient _httpClient;
        private readonly IAppSettings _appsSetting;
        private readonly HealthService _healthService;
        public TokenService(AppSettings appsSetting, HttpClient httpClient, HealthService healthService)
        {
            _appsSetting = appsSetting;
            _httpClient = httpClient;
            _healthService = healthService;
        }
        public async Task<AccessTokenDTO> GetAccessToken()
        {
            try
            {
                if (await _healthService.IsHealthUp())
                {
                    List<KeyValuePair<string, string>> postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("content-type", "application/x-www-form-urlencoded"));
                    HttpRequestMessage httpRequest = new HttpRequestMessage();
                    StringBuilder stringBuilder = new StringBuilder();
                    var queryParam = stringBuilder
                                    .AppendFormat("grant_type={0}", _appsSetting["ClientCredential:grant_type"])
                                    .AppendFormat("&scope={0}", _appsSetting["ClientCredential:scope"])
                                    .AppendFormat("&client_id={0}", _appsSetting["ClientCredential:client_id"])
                                    .AppendFormat("&client_secret={0}", _appsSetting["ClientCredential:client_secret"])
                                    .ToString();
                    httpRequest.RequestUri = new Uri($"{_appsSetting["DegreeVerifyEndPoints:Token"]}?{queryParam}");
                    httpRequest.Headers.Accept.Clear();
                    httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpRequest.Headers.Add("cache-control", "no-cache");
                    httpRequest.Method = HttpMethod.Post;
                    httpRequest.Content = new FormUrlEncodedContent(postData);
                    var response = await _httpClient.SendAsync(httpRequest);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        //return await response.Content.ReadFromJsonAsync<AccessTokenDTO>();
                        return await response.Content.ReadFromJsonAsync<AccessTokenDTO>();  
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }
    }
}
