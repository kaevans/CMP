using CMP.Core.Models;

namespace CMP.Infrastructure.Git
{
    public interface IGitRepository
    {
        Task<GitRepository> GetRepository();
    }
}
