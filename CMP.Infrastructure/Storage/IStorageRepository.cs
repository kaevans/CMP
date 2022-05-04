namespace CMP.Infrastructure.Storage
{
    public interface IStorageRepository
    {
        public Task<string> Get(string id);
        public Task Delete(string id);
        public Task<string> Add(string content);

    }
}
