namespace Prenoto.BookingLogic
{
    public class BookingConfirmation
    {
        public BookingConfirmation(bool success, string message, string pnr)
        {
            Success = success;
            Message = message;
            PNR = pnr;
        }
        public static BookingConfirmation Confirmed(string message, string pnr)
        {
            return new BookingConfirmation(true, message, pnr);
        }
        public static BookingConfirmation Failed(string message)
        {
            return new BookingConfirmation(false, message, null);
        }
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? PNR { get; set; }
    }
}
