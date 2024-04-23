using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Library.Elasticsearch.Entities;

namespace Library.Elasticsearch
{
    public interface IElasticsearchRepository<T, TKey> where T : class, IEntity<TKey>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(TKey id);
        Task InsertAsync(T document);
        Task UpdateAsync(T document);
        Task DeleteAsync(TKey id);
        Task DeleteByQueryAsync(TermsQuery querySelector);
        Task BulkInsertAsync(IEnumerable<T> documents);
        Task<IEnumerable<T>> GetByTermQueryAsync(TermQuery querySelector);
        Task<IEnumerable<T>> GetByQueryAsync(SearchRequestDescriptor<T> querySelector);
        Task<SearchResponse<T>> GetByQuerySearchAsync(SearchRequestDescriptor<T> querySelector);
        Task<long> ExistByTermQueryAsync(CountRequestDescriptor<T> querySelector);
        Task<List<string>> GetMappingInfoAsync(string propertyName);
    }
}
