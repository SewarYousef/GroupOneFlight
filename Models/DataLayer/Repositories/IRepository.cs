using System.Collections.Generic;

namespace GroupOneFlight.Models.DataLayer.Repositories
{
    public interface IRepository<T> where T : class
    {
        // Query
        IEnumerable<T> List(QueryOptions<T> options);
        T? Get(QueryOptions<T> options);
        T? Get(int id);
        int Count { get; }

        // Commands
        void Insert(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Save();
    }
}
