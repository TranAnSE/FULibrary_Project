using System.ComponentModel.DataAnnotations;

namespace LibraryManagementClient.Models;

public class SystemSettingsDto
{
    public Guid Id { get; set; }
    public Guid LibraryId { get; set; }
    public string LibraryName { get; set; } = string.Empty;
    
    [Display(Name = "Max Books Per Borrower")]
    [Range(1, 20, ErrorMessage = "Value must be between 1 and 20")]
    public int MaxBooksPerBorrower { get; set; }
    
    [Display(Name = "Loan Duration (Days)")]
    [Range(1, 90, ErrorMessage = "Value must be between 1 and 90")]
    public int LoanDurationDays { get; set; }
    
    [Display(Name = "Max Renewals")]
    [Range(0, 5, ErrorMessage = "Value must be between 0 and 5")]
    public int MaxRenewals { get; set; }
    
    [Display(Name = "Renewal Days")]
    [Range(1, 30, ErrorMessage = "Value must be between 1 and 30")]
    public int RenewalDays { get; set; }
    
    [Display(Name = "Daily Fine Rate")]
    [Range(0, 100, ErrorMessage = "Value must be between 0 and 100")]
    public decimal DailyFineRate { get; set; }
    
    [Display(Name = "Lost Book Fine Percent")]
    [Range(0, 200, ErrorMessage = "Value must be between 0 and 200")]
    public decimal LostBookFinePercent { get; set; }
    
    [Display(Name = "Reservation Expiry (Days)")]
    [Range(1, 30, ErrorMessage = "Value must be between 1 and 30")]
    public int ReservationExpiryDays { get; set; }
}

public class UpdateSystemSettingsDto
{
    public Guid Id { get; set; }
    public int MaxBooksPerBorrower { get; set; }
    public int LoanDurationDays { get; set; }
    public int MaxRenewals { get; set; }
    public int RenewalDays { get; set; }
    public decimal DailyFineRate { get; set; }
    public decimal LostBookFinePercent { get; set; }
    public int ReservationExpiryDays { get; set; }
}
