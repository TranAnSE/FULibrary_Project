using System.ComponentModel.DataAnnotations;

namespace LibraryManagementClient.Models;

public class ShelfLocationDto
{
    public Guid Id { get; set; }
    
    [Required(ErrorMessage = "Code is required")]
    [StringLength(20, ErrorMessage = "Code cannot exceed 20 characters")]
    public string Code { get; set; } = string.Empty;
    
    [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
    public string? Description { get; set; }
    
    public Guid LibraryId { get; set; }
    public string LibraryName { get; set; } = string.Empty;
}

public class CreateShelfLocationDto
{
    [Required(ErrorMessage = "Code is required")]
    public string Code { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    [Required(ErrorMessage = "Library is required")]
    public Guid LibraryId { get; set; }
}
