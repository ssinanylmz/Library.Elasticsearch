using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Bulk;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch.Mapping;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Transport.Products.Elasticsearch;
using Library.Elasticsearch.Entities;

namespace Library.Elasticsearch
{
    public class ElasticsearchRepository<T, TKey> : IElasticsearchRepository<T, string> where T : class, IEntity<string>
    {
        private readonly ElasticsearchClient _client;
        private readonly string _indexName;

        public ElasticsearchRepository(ElasticsearchClient client)
        {
            _client = client;
            _indexName = GetIndexName();
        }

        private string GetIndexName()
        {
            return typeof(T).Name.ToLower();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var response = await Execute(async () => await _client.SearchAsync<T>(s => s.Query(q => q.MatchAll()).Index(_indexName)));
            return response.Documents;
        }

        public async Task<T> GetByIdAsync(string id)
        {
            var response = await Execute(async () => await _client.GetAsync<T>(_indexName, id));
            return response?.Source;
        }

        public async Task InsertAsync(T document)
        {
            var response = await Execute(async () => await _client.IndexAsync(document, _indexName));
        }
        public async Task UpdateAsync(T document)
        {
            var response = await Execute(async () => await _client.UpdateAsync<T, T>(_indexName, document.Id.ToString(), x => x.Doc(document)));
        }
        public async Task DeleteAsync(string id)
        {
            await Execute(async () => await _client.DeleteAsync<T>(_indexName, id.ToString(), null));
        }
        public async Task DeleteByQueryAsync(TermsQuery querySelector)
        {
            await Execute(async () => await _client.DeleteByQueryAsync<T>(_indexName, q => q.Query(querySelector)));
        }
        public async Task BulkInsertAsync(IEnumerable<T> documents)
        {
            var bulkRequest = new BulkRequest { Operations = new List<IBulkOperation>() };
            foreach (var doc in documents)
            {
                bulkRequest.Operations.Add(new BulkIndexOperation<T>(doc, _indexName));
            }
            await Execute(async () => await _client.BulkAsync(bulkRequest));
        }

        public async Task<IEnumerable<T>> GetByTermQueryAsync(TermQuery querySelector)
        {
            var response = await Execute(async () => await _client.SearchAsync<T>(s => s.Index(_indexName).Query(querySelector)));
            foreach (var hit in response.Hits)
            {
                hit.Source.Id = hit.Id;
            }
            return response.Documents;
        }
        public async Task<IEnumerable<T>> GetByQueryAsync(SearchRequestDescriptor<T> querySelector)
        {
            var response = await Execute(async () => await _client.SearchAsync<T>(querySelector.Index(_indexName)));
            return response.Documents;
        }
        public async Task<SearchResponse<T>> GetByQuerySearchAsync(SearchRequestDescriptor<T> querySelector)
        {
            var response = await Execute(async () => await _client.SearchAsync<T>(querySelector.Index(_indexName)));
            return response;
        }
        public async Task<long> ExistByTermQueryAsync(CountRequestDescriptor<T> querySelector)
        {
            var response = await Execute(async () => await _client.CountAsync<T>(querySelector.Indices(_indexName)));

            return response.Count;
        }
        public async Task<List<string>> GetMappingInfoAsync(string propertyName)
        {
            var getMappingResponse = await Execute(async () => await _client.Indices.GetMappingAsync(new GetMappingRequest(_indexName)));

            var subPropertyNames = new List<string>();
            if (getMappingResponse.IsValidResponse && getMappingResponse.Indices.TryGetValue(_indexName, out var indexMapping))
            {
                if (indexMapping.Mappings.Properties!.TryGetProperty(propertyName, out var property))
                {
                    if (property is ObjectProperty objectProperty && objectProperty.Properties != null)
                    {
                        // Properties üzerinde döngü yaparak isimleri topla
                        foreach (var subProperty in objectProperty.Properties)
                        {
                            subPropertyNames.Add(subProperty.Key.ToString());
                        }
                    }
                }
            }

            return subPropertyNames;
        }
        private async Task<TResponse> Execute<TResponse>(Func<Task<TResponse>> action) where TResponse : ElasticsearchResponse
        {
            try
            {
                var response = await action();

                if (!response.IsValidResponse && !response.ApiCallDetails.HasSuccessfulStatusCode)
                {
                    throw new Exception($"Elasticsearch error: {response.ElasticsearchServerError?.Error?.Reason ?? "Unknown error"}");
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"Elasticsearch error: {ex}");
            }

        }
    }
}
