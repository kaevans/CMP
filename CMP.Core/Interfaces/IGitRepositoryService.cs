using CMP.Core.Models;

namespace CMP.Core.Interfaces
{
    public interface IGitRepositoryService
    {
        Task<GitRepository> GetRepository();
    }
}
