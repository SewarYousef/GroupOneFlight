using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace GroupOneFlight.Models.DataLayer.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected FlightDbContext context;
        private DbSet<T> dbSet;

        public Repository(FlightDbContext ctx)
        {
            context = ctx;
            dbSet   = context.Set<T>();
        }

        public int Count => dbSet.Count();

        // ── Query ───────────────────────────────────────────────────

        public IEnumerable<T> List(QueryOptions<T> options)
        {
            IQueryable<T> query = dbSet;

            foreach (string include in options.GetIncludes())
                query = query.Include(include);

            if (options.HasWhere)
                query = query.Where(options.Where!);

            if (options.HasOrderBy)
                query = query.OrderBy(options.OrderBy!);
            else if (options.HasOrderByDescending)
                query = query.OrderByDescending(options.OrderByDescending!);

            return query.ToList();
        }

        public T? Get(QueryOptions<T> options)
        {
            IQueryable<T> query = dbSet;

            foreach (string include in options.GetIncludes())
                query = query.Include(include);

            if (options.HasWhere)
                query = query.Where(options.Where!);

            return query.FirstOrDefault();
        }

        public T? Get(int id) => dbSet.Find(id);

        // ── Commands ────────────────────────────────────────────────

        public void Insert(T entity) => dbSet.Add(entity);
        public void Update(T entity) => dbSet.Update(entity);
        public void Delete(T entity) => dbSet.Remove(entity);
        public void Save()           => context.SaveChanges();
    }
}
