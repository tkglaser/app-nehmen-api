using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace app_nehmen_api.Services
{
    public class CosmosDbService : ICosmosDbService
    {
        private Container _container;
        private Database _database;

        public CosmosDbService(
            CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            this._database = dbClient.GetDatabase(databaseName);
            this._container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task<string> GetResourceToken(string userid)
        {
            var user = await _database.CreateUserAsync(userid);
            var permissionId = $"permission{userid}";
            var permission = await user.User.CreatePermissionAsync(new PermissionProperties(
                id: permissionId,
                permissionMode: PermissionMode.All,
                container: _container,
                resourcePartitionKey: new PartitionKey(userid)));
            return permission.Resource.Token;
        }
    }
}