using System.Threading.Tasks;

namespace FutbolApi.Services
{
    public interface IFetchDataService
    {
        Task FetchDataAsync(string url, string operationName);
    }
}
