using DegreeVerify.Client.Services.IServices;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace DegreeVerify.Client.Services
{
    public class AppSettings : IAppSettings
    {
        private readonly IConfiguration _configuration;
        public AppSettings(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string this[string key]
        {
            get
            {
                return _configuration[key];
            }
        }
        public Dictionary<string, string> Section(string key)
        {
            Dictionary<string, string> section = new Dictionary<string, string>();
            try
            {
                section = _configuration.GetSection(key).Get<Dictionary<string, string>>();
            }
            catch (Exception ex)
            {
            }
            return section;
        }
    }
}
