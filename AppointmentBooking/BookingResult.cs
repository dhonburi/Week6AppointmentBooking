namespace ENSE707_AppointmentBooking;

public class BookingResult
{
    public bool Success { get; }
    public string Message { get; }

    public BookingResult(bool success, string message)
    {
        Success = success;
        Message = message;
    }
}
