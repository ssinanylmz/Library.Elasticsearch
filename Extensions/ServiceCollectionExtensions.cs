using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Library.Elasticsearch.Settings;

namespace Library.Elasticsearch.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddElasticsearch(this IServiceCollection services,  IConfiguration configuration)
        {
            // Connection pool için birden fazla node tanımı
            var NodeUris = configuration.GetSection("Elasticsearch:NodeUris").Get<List<string>>();
            var fingerPrint = configuration["Elasticsearch:FingerPrint"];
            var apiKey = configuration["Elasticsearch:ApiKey"];

            var uris = NodeUris.Select(uri => new Uri(uri)).ToList();

            var connectionPool = new StickyNodePool(uris);

            var clientSettings = new ElasticsearchClientSettings(connectionPool).CertificateFingerprint(fingerPrint).Authentication(new ApiKey(apiKey));

            var client = new ElasticsearchClient(clientSettings);
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    var response = client.Ping();

                    if (!response.IsValidResponse)
                    {
                        Console.WriteLine(response.DebugInformation);
                    }
                    Console.WriteLine(response.DebugInformation);
                }

            }
            catch (Exception ex)
            {

                throw;
            }
      

            services.AddSingleton(client);

            services.AddTransient(typeof(IElasticsearchRepository<,>), typeof(ElasticsearchRepository<,>));

            return services;
        }
    }
}
