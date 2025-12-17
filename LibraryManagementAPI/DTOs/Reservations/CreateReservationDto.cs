namespace LibraryManagementAPI.DTOs.Reservations;

public class CreateReservationDto
{
    public Guid UserId { get; set; }
    public Guid BookId { get; set; }
}
