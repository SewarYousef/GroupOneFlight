namespace GroupOneFlight.Areas.Airlines.Models
{
    public static class AircraftTypes
    {
        // Airbus A320 family
        public const string AirbusA318 = "Airbus A318";
        public const string AirbusA319 = "Airbus A319";
        public const string AirbusA320 = "Airbus A320";
        public const string AirbusA321 = "Airbus A321";

        // Boeing 737 family
        public const string Boeing737_700 = "Boeing 737-700";
        public const string Boeing737_800 = "Boeing 737-800";
        public const string Boeing737_900 = "Boeing 737-900";
        public const string Boeing737Max8 = "Boeing 737 MAX 8";

        public static List<string> GetAll() => new()
        {
            AirbusA318, AirbusA319, AirbusA320, AirbusA321,
            Boeing737_700, Boeing737_800, Boeing737_900, Boeing737Max8
        };
    }
}
