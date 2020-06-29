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
            var user = await GetOrCreateUser(userid);
            var permission = await GetOrCreatePermission(user);
            return permission.Token;
        }

        private async Task<User> GetOrCreateUser(string userid)
        {
            User user;

            try
            {
                var userResponse = await _database.GetUser(userid).ReadAsync();
                user = userResponse.User;
            }
            catch (CosmosException x)
            {
                if (x.StatusCode == HttpStatusCode.NotFound)
                {
                    var userResponse = await _database.CreateUserAsync(userid);
                    user = userResponse.User;
                }
                else
                {
                    throw;
                };
            }

            return user;
        }

        private async Task<PermissionProperties> GetOrCreatePermission(User user)
        {
            PermissionProperties permission;
            var permissionId = $"permission{user.Id}";

            try
            {
                var permissionResponse = await user.GetPermission(permissionId).ReadAsync();
                permission = permissionResponse.Resource;
            }
            catch (CosmosException x)
            {
                if (x.StatusCode == HttpStatusCode.NotFound)
                {
                    var permissionResponse = await user.CreatePermissionAsync(new PermissionProperties(
                        id: permissionId,
                        permissionMode: PermissionMode.All,
                        container: _container,
                        resourcePartitionKey: new PartitionKey(user.Id)));
                    permission = permissionResponse.Resource;
                }
                else
                {
                    throw;
                }
            }

            return permission;
        }
    }
}