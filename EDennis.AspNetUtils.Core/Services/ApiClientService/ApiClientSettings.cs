namespace EDennis.AspNetUtils
{

    public class ApiClientSettingsDictionary: Dictionary<string, ApiClientSettings> { }

    public class ApiClientSettings
    {
        public const string DefaultConfigKey = "Apis";
        public string ApiClientName { get; set; }
        public string BaseAddress { get; set; }
        public Dictionary<string,string> Properties { get; set; }
    }
}
