using CMP.Core.Models;
using Microsoft.Extensions.Logging;


namespace CMP.Infrastructure.Git
{
    public interface IGitRepoItemRepositoryFactory<T> where T : GitRepoItem
    {
        IGitRepoItemRepository<T> GetRepoItemRepository(GitRepo repo, ILogger logger);
    }
}
