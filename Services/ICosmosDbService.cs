using System.Threading.Tasks;

namespace app_nehmen_api.Services
{
    public interface ICosmosDbService
    {
        Task<string> GetResourceToken(string userid);
    }
}