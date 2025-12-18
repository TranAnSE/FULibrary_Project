namespace LibraryManagementClient.Models;

public class ReservationDto
{
    public Guid Id { get; set; }
    public DateTime ReservationDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public DateTime? FulfilledDate { get; set; }
    public DateTime? CancelledDate { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = string.Empty;
    
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    
    public Guid BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string? BookCoverImageUrl { get; set; }
}

public class CreateReservationDto
{
    public Guid UserId { get; set; }
    public Guid BookId { get; set; }
}
