namespace GroupOneFlight.Models.DomainModels
{
    public static class CabinTypes
    {
        public const string BasicEconomy = "Basic Economy";
        public const string Economy      = "Economy";
        public const string EconomyPlus  = "Economy Plus";
        public const string Business     = "Business";
 
        public static List<string> GetAll() => new()
        {
            BasicEconomy,
            Economy,
            EconomyPlus,
            Business
        };
    }
}
