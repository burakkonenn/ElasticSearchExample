using ElasticSearchExample.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElasticSearchExample.Interfaces
{
    public interface IElasticSearchService
    {
        Task CheckIndex(string indexName);
        Task InsertDocument(string indexName, Product product);
        Task InsertDocuments(string indexName, List<Product> product);
        Task<Product> GetDocument(string indexName, int id);
        Task<List<Product>> GetDocuments(string indexName);
    }
}
