using ElasticSearchExample.Entities;
using ElasticSearchExample.Interfaces;
using ElasticSearchExample.Mapping;
using Microsoft.Extensions.Configuration;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchExample.Concrete
{
    public class ElasticSearchService : IElasticSearchService
    {

        private readonly IConfiguration _configuration;
        private readonly IElasticClient _client;

        public ElasticSearchService(IConfiguration configuration)
        {
            _configuration = configuration;
            _client = CreateInstance();
        }


        private ElasticClient CreateInstance()
        {
            string host = _configuration.GetSection("ElasticsearchServer:Host").Value;
            string port = _configuration.GetSection("ElasticsearchServer:Port").Value;
            string username = _configuration.GetSection("ElasticsearchServer:Username").Value;
            string password = _configuration.GetSection("ElasticsearchServer:Password").Value;

            var settings = new ConnectionSettings(new Uri(host + ":" + port));

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                settings.BasicAuthentication(username, password);

            return new ElasticClient(settings);


        }


        public async Task CheckIndex(string indexName)
        {
            var anyy = await _client.Indices.ExistsAsync(indexName);
            if (anyy.Exists)
                return;

            var response = await _client.Indices.CreateAsync(indexName, c =>
                        c.Index(indexName)
                        .ProductMapping()
                        .Settings(s => s.NumberOfShards(3).NumberOfReplicas(1)));
        }



        public async Task InsertDocument(string indexName, Product product)
        {
            
            ///eğer mevcut döküman daha önce eklenmişse güncelleme yapar.


            var response = await _client.CreateAsync(product, q => q.Index(indexName));
            if(response.ApiCall?.HttpStatusCode == 409)
            {
                await _client.UpdateAsync<Product>(response.Id, a => a.Index(indexName).Doc(product));
            }
        }

        public async Task InsertDocuments(string indexName, List<Product> product)
        {
            ///dökümanlar toplu bir şekilde eklenecektir.
            await _client.IndexManyAsync(product, index: indexName);
        }

        public async Task<Product> GetDocument(string indexName, int id)
        {
            //Tek bir döküman çekilir.
            var response = await _client.GetAsync<Product>(id, q => q.Index(indexName));

            return response.Source;
        }

        public async Task<List<Product>> GetDocuments(string indexName)
        {
            var responses = await _client.SearchAsync<Product>(q => q.Index(indexName).Scroll("5m"));

            return responses.Documents.ToList();
        }
    }
}
