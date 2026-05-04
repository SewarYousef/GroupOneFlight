using System;
using System.Linq.Expressions;

namespace GroupOneFlight.Models.DataLayer.Repositories
{
    public class QueryOptions<T>
    {
        // WHERE clause
        public Expression<Func<T, bool>>? Where { get; set; }
        public bool HasWhere => Where != null;

        // ORDER BY clause
        public Expression<Func<T, object>>? OrderBy { get; set; }
        public bool HasOrderBy => OrderBy != null;

        // ORDER BY DESC
        public Expression<Func<T, object>>? OrderByDescending { get; set; }
        public bool HasOrderByDescending => OrderByDescending != null;

        // Navigation property includes  (comma-separated, e.g. "Airline,FlightOptions")
        private string _includes = string.Empty;
        public string Includes
        {
            get => _includes;
            set => _includes = value;
        }

        public string[] GetIncludes() =>
            _includes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
    }
}
