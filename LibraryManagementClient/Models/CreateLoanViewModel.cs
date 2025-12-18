using System.ComponentModel.DataAnnotations;

namespace LibraryManagementClient.Models;

public class CreateLoanViewModel
{
    public Guid? UserId { get; set; }
    
    [Display(Name = "Card Number")]
    public string? CardNumber { get; set; }
    
    public string? BorrowerName { get; set; }
    
    public Guid? BookCopyId { get; set; }
    
    [Display(Name = "Registration Number")]
    public string? RegistrationNumber { get; set; }
    
    public string? BookTitle { get; set; }
}
