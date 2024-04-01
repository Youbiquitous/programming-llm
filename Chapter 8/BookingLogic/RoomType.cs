namespace Prenoto.BookingLogic
{
    public class RoomType
    {
        public static RoomType Single() 
        {
            return new RoomType(
                "Single",
                "A simple single room description.",
                Price.Dollar(200));
        }
        public static RoomType Double()
        {
            return new RoomType(
                "Double",
                "A simple double room description.",
                Price.Dollar(300));
        }
        public RoomType(string name, string description, Price price)
        {
            Name = name;
            Description = description;
            PricePerNight = price;
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public Price PricePerNight { get; set; }
    }

    public class Price 
    {
        public Price(string currency, float amount)
        {
            Currency = currency;
            Amount = amount;
        }
        public static Price Dollar(float amount)
        {
            return new Price("USD", amount);
        }
        public string Currency { get; set; }
        public float Amount { get; set; }
    }
}
