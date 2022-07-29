using CMP.Core.Interfaces;
using CMP.Core.Models;

namespace CMP.Infrastructure.Git
{
    public interface IGitRepoItemRepository<T> : IRepository<T> where T : GitRepoItem
    {
    }
}
