namespace Prenoto.BookingLogic
{
    public class Availability
    {
        public Availability(RoomType room, int numberOfRooms, int numberOfNights)
        {
            RoomType = room;
            NumberOfRooms = numberOfRooms;
            NumberOfNights = numberOfNights;
        }
        public RoomType RoomType { get; set; }
        public int NumberOfRooms { get; set; }
        public int NumberOfNights { get; set; }
        public Price TotalPrice => new (RoomType.PricePerNight.Currency, RoomType.PricePerNight.Amount * NumberOfNights);
    }
}
