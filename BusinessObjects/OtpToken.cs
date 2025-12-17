namespace BusinessObjects;

public class OtpToken : BaseEntity
{
    public string Code { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime ExpiryDate { get; set; }
    public bool IsUsed { get; set; }
    public int AttemptCount { get; set; }
}
