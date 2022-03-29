using CMP.Functions.Models;
using System.Threading.Tasks;

namespace CMP.Functions.Services
{
    public interface IGitRepositoryService
    {
        Task<GitRepository> GetRepository();
    }
}
