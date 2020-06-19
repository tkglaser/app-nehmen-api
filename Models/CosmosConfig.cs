namespace app_nehmen_api.Models
{
    public class CosmosConfig
    {
        public const string CosmosDb = "CosmosDb";
        public string Account { get; set; }
        public string Key { get; set; }
        public string DatabaseName { get; set; }
        public string ContainerName { get; set; }
    }
}