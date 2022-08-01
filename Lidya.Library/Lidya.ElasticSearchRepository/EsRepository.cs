using Elasticsearch.Net;
using Microsoft.Extensions.Configuration;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lidya.ElasticSearchRepository
{
    public class EsRepository<T> : IEsRepository<T>
    where T : class
    {
        readonly ElasticClient _client;
        readonly ElasticClient _clasterClient;

        public string Index { get; }

        public EsRepository(Uri[] nodes,
            string index,
            string username,
            string password)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                ConnectionSettings connectionSettings;
                ElasticClient elasticClient;
                StaticConnectionPool connectionPool;
                connectionPool = new StaticConnectionPool(new Uri[] { nodes[i] });
                connectionSettings = new ConnectionSettings(connectionPool);
                connectionSettings.DisableDirectStreaming();
                connectionSettings.DefaultIndex(index);
                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    connectionSettings.BasicAuthentication(username, password);
                }
                connectionSettings.ConnectionLimit(-1);
                connectionSettings.DisablePing();
                connectionSettings.DisableAutomaticProxyDetection(false);
                elasticClient = new ElasticClient(connectionSettings);
                if (i == 0)
                {
                    _client = elasticClient;
                }
                else if (i == 1)
                {
                    _clasterClient = elasticClient;
                }

            }

            Index = index;
        }

        public EsRepository(IConfiguration Configuration)
        {
            Uri[] nodes = Configuration.GetSection("elasticsearch:nodes").Value.Split(';').Select(s => new Uri(s)).ToArray();
            string index = Configuration.GetSection($"elasticsearch:{typeof(T).Name.ToLower()}_index").Value;
            Index = index;
            string username = Configuration.GetSection("elasticsearch:username").Value;
            string password = Configuration.GetSection("elasticsearch:password").Value;
            for (int i = 0; i < nodes.Length; i++)
            {
                ConnectionSettings connectionSettings;
                ElasticClient elasticClient;
                StaticConnectionPool connectionPool;
                connectionPool = new StaticConnectionPool(new Uri[] { nodes[i] });
                connectionSettings = new ConnectionSettings(connectionPool);
                connectionSettings.DisableDirectStreaming();
                connectionSettings.DefaultIndex(index);
                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    connectionSettings.BasicAuthentication(username, password);
                }
                connectionSettings.ConnectionLimit(-1);
                connectionSettings.DisablePing();
                connectionSettings.DisableAutomaticProxyDetection(false);
                elasticClient = new ElasticClient(connectionSettings);
                if (i == 0)
                {
                    _client = elasticClient;
                }
                else if (i == 1)
                {
                    _clasterClient = elasticClient;
                }

            }
        }

        public T GetById(string id)
        {
            var response = _client.Get<T>(id);
            if (response.OriginalException != null)
            {
                throw response.OriginalException;
            }
            return response.Source;
        }

        public async Task<T> GetByIdAsync(string id)
        {
            var response = await _client.GetAsync<T>(id);
            if (response.OriginalException != null)
            {
                throw response.OriginalException;
            }
            return response.Source;
        }

        #region Insert
        public bool Insert(T model)
        {
            if (_clasterClient != null)
            {
                var clasterResponse = _clasterClient.IndexDocument(model);
                IsError(clasterResponse);
            }

            var response = _client.IndexDocument(model);
            IsError(response);
            return response.IsValid;
        }

        public async Task<bool> InsertAsync(T model)
        {
            if (_clasterClient != null)
            {
                var clasterResponse = await _clasterClient.IndexDocumentAsync(model);
                IsError(clasterResponse);
            }

            var response = await _client.IndexDocumentAsync(model);
            IsError(response);
            return response.IsValid;
        }

        public bool InsertMany(List<T> models)
        {
            if (_clasterClient != null)
            {
                var clasterRespose = _clasterClient.IndexMany(models);
                IsError(clasterRespose);
            }

            var response = _client.IndexMany(models);
            IsError(response);
            return response.IsValid;
        }

        public async Task<bool> InsertManyAsync(List<T> models)
        {
            if (_clasterClient != null)
            {
                var clasterResponse = await _clasterClient.IndexManyAsync(models);
                IsError(clasterResponse);
            }

            var response = await _client.IndexManyAsync(models);
            IsError(response);
            return response.IsValid;
        }

        #endregion

        #region update
        public bool Update(string id, T model)
        {
            if (_clasterClient != null)
            {
                var clasterResponse = _clasterClient.Update<T>(id, x => x.Doc(model).Refresh(Refresh.True));
                IsError(clasterResponse);
            }

            var response = _client.Update<T>(id, x => x.Doc(model).Refresh(Refresh.True));
            IsError(response);
            return response.IsValid;
        }

        public async Task<bool> UpdateAsync(string id, T model)
        {
            if (_clasterClient != null)
            {
                var clasterResponse = await _clasterClient.UpdateAsync<T>(id, x => x.Doc(model).Refresh(Refresh.True));
                IsError(clasterResponse);
            }

            var response = await _client.UpdateAsync<T>(id, x => x.Doc(model).Refresh(Refresh.True));
            IsError(response);
            return response.IsValid;
        }

        public bool Update<TPartial>(string id, TPartial model) where TPartial : class
        {
            if (_clasterClient != null)
            {
                var clasterResponse = _clasterClient.Update<T, TPartial>(id, x => x.Doc(model).Refresh(Refresh.True));
                IsError(clasterResponse);
            }

            var response = _client.Update<T, TPartial>(id, x => x.Doc(model).Refresh(Refresh.True));
            IsError(response);
            return response.IsValid;
        }

        public async Task<bool> UpdateAsync<TPartial>(string id, TPartial model) where TPartial : class
        {
            if (_clasterClient != null)
            {
                var clasterResponse = await _clasterClient.UpdateAsync<T, TPartial>(id, x => x.Doc(model).Refresh(Refresh.True));
                IsError(clasterResponse);
            }

            var response = await _client.UpdateAsync<T, TPartial>(id, x => x.Doc(model).Refresh(Refresh.True));
            IsError(response);
            return response.IsValid;
        }


        public bool UpdateByQuery(Func<UpdateByQueryDescriptor<T>, IUpdateByQueryRequest> selector)
        {
            if (_clasterClient != null)
            {
                var clasterResponse = _clasterClient.UpdateByQuery<T>(selector);
                IsError(clasterResponse);
            }

            var response = _client.UpdateByQuery<T>(selector);
            IsError(response);
            return response.IsValid;
        }

        public async Task<bool> UpdateByQueryAsync(Func<UpdateByQueryDescriptor<T>, IUpdateByQueryRequest> selector)
        {
            if (_clasterClient != null)
            {
                var clasterResponse = await _clasterClient.UpdateByQueryAsync<T>(selector);
                IsError(clasterResponse);
            }

            var response = await _client.UpdateByQueryAsync<T>(selector);
            IsError(response);
            return response.IsValid;
        }

        #endregion

        public bool Bulk(BulkDescriptor bulk)
        {
            if (_clasterClient != null)
            {
                var clasterResponse = _clasterClient.Bulk(bulk);
                IsError(clasterResponse);
            }

            var response = _client.Bulk(bulk);
            IsError(response);
            return !response.Errors && response.IsValid;
        }

        public async Task<bool> BulkAsync(BulkDescriptor bulk)
        {
            if (_clasterClient != null)
            {
                var clasterResponse = await _clasterClient.BulkAsync(bulk);
                IsError(clasterResponse);
            }

            var response = await _client.BulkAsync(bulk);
            IsError(response);
            return !response.Errors && response.IsValid;
        }

        #region Search
        public ISearchResponse<T> Search(ISearchRequest<T> request)
        {
            return _client.Search<T>(request);
        }

        public ISearchResponse<T> Search(Func<SearchDescriptor<T>, ISearchRequest> selector = null)
        {
            return _client.Search<T>(selector);
        }

        public Task<ISearchResponse<T>> SearchAsync(Func<SearchDescriptor<T>, ISearchRequest> selector = null)
        {
            return _client.SearchAsync<T>(selector);
        }

        public ISearchResponse<T> Scroll(Time scrollTime, string scrollId, Func<ScrollDescriptor<T>, IScrollRequest> selector = null)
        {
            return _client.Scroll<T>(scrollTime, scrollId, selector);
        }

        public bool ClearScroll(string scrollId)
        {
            return _client.ClearScroll(new ClearScrollRequest(scrollId)).IsValid;
        }

        public Task<ISearchResponse<T>> ScrollAsync(Time scrollTime, string scrollId, Func<ScrollDescriptor<T>, IScrollRequest> selector = null)
        {
            return _client.ScrollAsync<T>(scrollTime, scrollId, selector);
        }

        public async Task<bool> ClearScrollAsync(string scrollId)
        {
            return (await _client.ClearScrollAsync(new ClearScrollRequest(scrollId))).IsValid;
        }
        #endregion

        #region Delete
        public bool DeleteByQuery(Func<QueryContainerDescriptor<T>, QueryContainer> querySelector)
        {
            if (_clasterClient != null)
            {
                var clasterResponse = _clasterClient.DeleteByQuery<T>(x => x.Query(querySelector));
                IsError(clasterResponse);
            }

            var response = _client.DeleteByQuery<T>(x => x.Query(querySelector));
            IsError(response);
            return response.IsValid;
        }

        public async Task<bool> DeleteByQueryAsync(Func<QueryContainerDescriptor<T>, QueryContainer> querySelector)
        {
            if (_clasterClient != null)
            {
                var clasterResponse = await _clasterClient.DeleteByQueryAsync<T>(x => x.Query(querySelector));
                IsError(clasterResponse);
            }

            var response = await _client.DeleteByQueryAsync<T>(x => x.Query(querySelector));
            IsError(response);
            return response.IsValid;
        }

        public bool Delete(string id)
        {
            if (_clasterClient != null)
            {
                var clasterResponse = _clasterClient.Delete<T>(id);
                IsError(clasterResponse);
            }

            var response = _client.Delete<T>(id);
            IsError(response);
            return response.IsValid;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            if (_clasterClient != null)
            {
                var clasterResponse = await _clasterClient.DeleteAsync<T>(id);
                IsError(clasterResponse);
            }

            var response = await _client.DeleteAsync<T>(id);
            IsError(response);
            return response.IsValid;
        }
        #endregion

        private void IsError(WriteResponseBase response)
        {
            if (response.Result == Result.Error)
            {
                if (response.OriginalException != null)
                {
                    throw response.OriginalException;
                }

                throw new Exception(response.ServerError.Error.ToString());
            }
        }
        private void IsError(IResponse response)
        {
            if (!response.IsValid)
            {
                if (response.OriginalException != null)
                {
                    throw response.OriginalException;
                }
            }
        }
    }
}

