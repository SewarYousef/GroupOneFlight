using Microsoft.AspNetCore.Http;

namespace GroupOneFlight.Areas.Airlines.Models
{
    public class FlightSession
    {
        private const string FilterKey    = "FlightFilter";
        private const string SelectionKey = "SelectedFlights";

        private readonly ISession _session;

        public FlightSession(ISession session)
        {
            _session = session;
        }

        public void SetFilter(FlightFilter filter)         => _session.SetObject(FilterKey, filter);
        public FlightFilter GetFilter()                    => _session.GetObject<FlightFilter>(FilterKey) ?? new FlightFilter();

        public void SetSelectedFlights(List<int> ids)      => _session.SetObject(SelectionKey, ids);
        public List<int> GetSelectedFlights()              => _session.GetObject<List<int>>(SelectionKey) ?? new List<int>();

        public void AddFlight(int id)
        {
            var ids = GetSelectedFlights();
            if (!ids.Contains(id)) { ids.Add(id); SetSelectedFlights(ids); }
        }

        public void RemoveFlight(int id)
        {
            var ids = GetSelectedFlights();
            ids.Remove(id);
            SetSelectedFlights(ids);
        }

        public void ClearSelections()  => _session.Remove(SelectionKey);
        public int SelectionCount      => GetSelectedFlights().Count;
    }
}
