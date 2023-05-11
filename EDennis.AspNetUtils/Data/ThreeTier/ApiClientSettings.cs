
namespace EDennis.AspNetUtils
{
    public class ApiClientSettingsDictionary : Dictionary<string, ApiClientSettings> { }
    public class ApiClientSettings
    {
        public string ApiKey { get; set; }
        public string BaseAddress { get; set; }
    }
}
