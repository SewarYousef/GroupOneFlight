namespace GroupOneFlight.Areas.Airlines.Models
{
    public static class CabinTypes
    {
        public const string Economy = "Economy";
        public const string Business = "Business";
        public const string FirstClass = "First Class";

        public static List<string> GetAllCabinTypes()
        {
            return new List<string>
            {
                Economy,
                Business,
                FirstClass
            };
        }
    }
}
