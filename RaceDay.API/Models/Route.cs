
namespace RaceDay.API.Models
{
    public class Route
    {
        public int RouteID { get; set; }

        public int EventID { get; set; }

        public string RouteName { get; set; } = string.Empty;

        public string? RouteDescription { get; set; }

        public string? RouteUrl { get; set; }

        public decimal Distance { get; set; }
    }
}
