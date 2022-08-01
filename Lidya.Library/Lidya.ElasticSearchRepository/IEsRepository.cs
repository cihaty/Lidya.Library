using Nest;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lidya.ElasticSearchRepository
{
    public interface IEsRepository<T>
        where T : class
    {
        string Index { get; }
        T GetById(string id);
        Task<T> GetByIdAsync(string id);
        ISearchResponse<T> Search(ISearchRequest<T> request);
        ISearchResponse<T> Search(Func<SearchDescriptor<T>, ISearchRequest> selector = null);
        Task<ISearchResponse<T>> SearchAsync(Func<SearchDescriptor<T>, ISearchRequest> selector = null);
        ISearchResponse<T> Scroll(Time scrollTime, string scrollId, Func<ScrollDescriptor<T>, IScrollRequest> selector = null);
        public bool ClearScroll(string scrollId);
        Task<ISearchResponse<T>> ScrollAsync(Time scrollTime, string scrollId, Func<ScrollDescriptor<T>, IScrollRequest> selector = null);
        Task<bool> ClearScrollAsync(string scrollId);
        bool Insert(T model);
        bool InsertMany(List<T> models);
        Task<bool> InsertAsync(T model);
        Task<bool> InsertManyAsync(List<T> models);
        bool Update(string id, T model);
        Task<bool> UpdateAsync(string id, T model);
        bool Update<TPartial>(string id, TPartial model) where TPartial : class;
        Task<bool> UpdateAsync<TPartial>(string id, TPartial model) where TPartial : class;
        bool UpdateByQuery(Func<UpdateByQueryDescriptor<T>, IUpdateByQueryRequest> selector);
        Task<bool> UpdateByQueryAsync(Func<UpdateByQueryDescriptor<T>, IUpdateByQueryRequest> selector);
        bool Bulk(BulkDescriptor bulk);
        Task<bool> BulkAsync(BulkDescriptor bulk);
        bool DeleteByQuery(Func<QueryContainerDescriptor<T>, QueryContainer> querySelector);
        Task<bool> DeleteByQueryAsync(Func<QueryContainerDescriptor<T>, QueryContainer> querySelector);
        bool Delete(string id);
        Task<bool> DeleteAsync(string id);
    }
}
