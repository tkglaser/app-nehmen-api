using System.Net;
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
            User user;
            try
            {
                var userResponse = await _database.CreateUserAsync(userid);
                user = userResponse.User;
            }
            catch (CosmosException ex)
            {
                if (ex.StatusCode == HttpStatusCode.Conflict)
                {
                    user = _database.GetUser(userid);
                }
                else
                {
                    throw;
                }
            }
            
            var permissionId = $"permission{userid}";

            PermissionProperties properties;

            try
            {
                var permissionResponse = await user.CreatePermissionAsync(new PermissionProperties(
                    id: permissionId,
                    permissionMode: PermissionMode.All,
                    container: _container,
                    resourcePartitionKey: new PartitionKey(userid)));
                properties = permissionResponse.Resource;
            }
            catch (CosmosException ex)
            {
                if (ex.StatusCode == HttpStatusCode.Conflict)
                {
                    var existingPermissionResponse = await user.GetPermission(permissionId).ReadAsync();
                    properties = existingPermissionResponse.Resource;
                }
                else
                {
                    throw;
                }
            }

            return properties.Token;
        }
    }
}