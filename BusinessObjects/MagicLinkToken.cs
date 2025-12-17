namespace BusinessObjects;

public class MagicLinkToken : BaseEntity
{
    public string Token { get; set; } = null!;
    public DateTime ExpiryDate { get; set; }
    public bool IsUsed { get; set; }
    public DateTime? UsedAt { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
