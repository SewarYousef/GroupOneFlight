using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace GroupOneFlight.Models.ExtensionMethods
{
    public class FlightCookie
    {
        private const string Key = "SelectedFlightsCookie";
        private static readonly TimeSpan Expiry = TimeSpan.FromDays(14);

        private readonly IHttpContextAccessor _accessor;
        private HttpContext HttpContext => _accessor.HttpContext!;

        public FlightCookie(IHttpContextAccessor accessor) => _accessor = accessor;

        public List<int> GetSelectedFlights()
        {
            var json = HttpContext.Request.Cookies[Key];
            if (string.IsNullOrEmpty(json)) return new List<int>();
            try { return JsonSerializer.Deserialize<List<int>>(json) ?? new List<int>(); }
            catch { return new List<int>(); }
        }

        public void SetSelectedFlights(List<int> ids)
        {
            HttpContext.Response.Cookies.Append(Key, JsonSerializer.Serialize(ids), new CookieOptions
            {
                Expires     = DateTimeOffset.UtcNow.Add(Expiry),
                HttpOnly    = true,
                IsEssential = true,
                SameSite    = SameSiteMode.Lax
            });
        }

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

        public void ClearSelections() => HttpContext.Response.Cookies.Delete(Key);
    }
}
