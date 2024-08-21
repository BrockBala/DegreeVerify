using DegreeVerify.Client.Services.IServices;
using DegreeVerify.DTO;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DegreeVerify.Client.Services
{
    public class HealthService
    {
        private readonly HttpClient _httpClient;
        private readonly IAppSettings _appsSetting;

        public HealthService(HttpClient httpClient, AppSettings appsSetting)
        {
            _httpClient = httpClient;
            _appsSetting = appsSetting;
        }

        public async Task<bool> IsHealthUp()
        {
            bool status = false;
            try
            {
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");

                var response = await _httpClient.GetAsync(_appsSetting["DegreeVerifyEndPoints:Health"]);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonConvert.DeserializeObject<HealthDTO>(responseContent);
                    if (jsonResponse?.Status == "UP")
                        status = true;
                }
                else
                {
                    // Http response error
                }
            }
            catch (Exception ex)
            {

            }
            return status;
        }
    }
}
