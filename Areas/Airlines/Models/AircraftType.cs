namespace GroupOneFlight.Areas.Airlines.Models
{
    public static class AircraftTypes
    {
        public const string Boeing737 = "Boeing 737";
        public const string Boeing787 = "Boeing 787";
        public const string Boeing777 = "Boeing 777";
        public const string AirbusA320 = "Airbus A320";
        public const string AirbusA380 = "Airbus A380";
        public const string Embraer190 = "Embraer E190";

        public static List<string> GetAll()
        {
            return new List<string>
            {
                Boeing737,
                Boeing787,
                Boeing777,
                AirbusA320,
                AirbusA380,
                Embraer190
            };
        }
    }
}
