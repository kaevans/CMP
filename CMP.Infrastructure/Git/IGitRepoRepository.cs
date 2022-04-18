using CMP.Core.Interfaces;
using CMP.Core.Models;

namespace CMP.Infrastructure.Git
{
    public interface IGitRepoRepository<T> : IRepository<T> where T : Entity
    {
        Task<GitRepository> GetRepositoryAsync();
    }
}
