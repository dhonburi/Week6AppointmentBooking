namespace ENSE707_AppointmentBooking;

public class AppointmentBookingService
{
    public BookingResult BookAppointment(AppointmentRequest? request)
    {
        if (request is null)
        {
            return new BookingResult(false, "Appointment request is missing.");
        }

        if (!request.Doctor.HasAvailableSlot())
        {
            return new BookingResult(
                false,
                $"Appointment cannot be booked because {request.Doctor.FullName} has no available slots.");
        }

        request.Doctor.ReserveSlot();

        return new BookingResult(
            true,
            $"Appointment booked successfully for {request.Patient.DisplayName} with {request.Doctor.FullName}.");
    }
}
