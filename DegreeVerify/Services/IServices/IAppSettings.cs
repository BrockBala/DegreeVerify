using System.Collections.Generic;

namespace DegreeVerify.Client.Services.IServices
{
    public interface IAppSettings
    {
        string this[string key] { get; }
        Dictionary<string, string> Section(string key);
    }
}
