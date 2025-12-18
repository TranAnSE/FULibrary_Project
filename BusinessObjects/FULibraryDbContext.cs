using Microsoft.EntityFrameworkCore;

namespace BusinessObjects;

public class FULibraryDbContext : DbContext
{
    public FULibraryDbContext(DbContextOptions<FULibraryDbContext> options) : base(options)
    {
    }

    public DbSet<Library> Libraries { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<Publisher> Publishers { get; set; }
    public DbSet<ShelfLocation> ShelfLocations { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<BookCopy> BookCopies { get; set; }
    public DbSet<Loan> Loans { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Fine> Fines { get; set; }
    public DbSet<FinePayment> FinePayments { get; set; }
    public DbSet<SystemSettings> SystemSettings { get; set; }
    public DbSet<MagicLinkToken> MagicLinkTokens { get; set; }
    public DbSet<OtpToken> OtpTokens { get; set; }
    public DbSet<BookPurchaseSuggestion> BookPurchaseSuggestions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureBaseEntity(modelBuilder);
        ConfigureRelationships(modelBuilder);
        ConfigureIndexes(modelBuilder);
        ConfigureDecimalPrecision(modelBuilder);
        ConfigureGlobalQueryFilters(modelBuilder);
    }

    private void ConfigureBaseEntity(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property<byte[]>("RowVersion")
                    .IsRowVersion()
                    .IsConcurrencyToken();

                modelBuilder.Entity(entityType.ClrType)
                    .Property<DateTime>("CreatedAt")
                    .HasDefaultValueSql("GETUTCDATE()");

                modelBuilder.Entity(entityType.ClrType)
                    .Property<DateTime>("UpdatedAt")
                    .HasDefaultValueSql("GETUTCDATE()");
            }
        }
    }

    private void ConfigureRelationships(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasOne(u => u.HomeLibrary)
            .WithMany(l => l.HomeBorrowers)
            .HasForeignKey(u => u.HomeLibraryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasOne(u => u.AssignedLibrary)
            .WithMany(l => l.AssignedLibrarians)
            .HasForeignKey(u => u.AssignedLibraryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Book>()
            .HasOne(b => b.Library)
            .WithMany(l => l.Books)
            .HasForeignKey(b => b.LibraryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Book>()
            .HasOne(b => b.Category)
            .WithMany(c => c.Books)
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Book>()
            .HasOne(b => b.Language)
            .WithMany(l => l.Books)
            .HasForeignKey(b => b.LanguageId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Book>()
            .HasOne(b => b.Publisher)
            .WithMany(p => p.Books)
            .HasForeignKey(b => b.PublisherId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<BookCopy>()
            .HasOne(bc => bc.Book)
            .WithMany(b => b.Copies)
            .HasForeignKey(bc => bc.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BookCopy>()
            .HasOne(bc => bc.ShelfLocation)
            .WithMany(sl => sl.BookCopies)
            .HasForeignKey(bc => bc.ShelfLocationId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ShelfLocation>()
            .HasOne(sl => sl.Library)
            .WithMany(l => l.ShelfLocations)
            .HasForeignKey(sl => sl.LibraryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Loan>()
            .HasOne(l => l.User)
            .WithMany(u => u.Loans)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Loan>()
            .HasOne(l => l.BookCopy)
            .WithMany(bc => bc.Loans)
            .HasForeignKey(l => l.BookCopyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reservations)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.Book)
            .WithMany(b => b.Reservations)
            .HasForeignKey(r => r.BookId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Fine>()
            .HasOne(f => f.User)
            .WithMany(u => u.Fines)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Fine>()
            .HasOne(f => f.Loan)
            .WithMany(l => l.Fines)
            .HasForeignKey(f => f.LoanId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<FinePayment>()
            .HasOne(fp => fp.Fine)
            .WithMany(f => f.Payments)
            .HasForeignKey(fp => fp.FineId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SystemSettings>()
            .HasOne(ss => ss.Library)
            .WithMany(l => l.Settings)
            .HasForeignKey(ss => ss.LibraryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BookPurchaseSuggestion>()
            .HasOne(bps => bps.User)
            .WithMany(u => u.BookSuggestions)
            .HasForeignKey(bps => bps.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<BookPurchaseSuggestion>()
            .HasOne(bps => bps.Library)
            .WithMany()
            .HasForeignKey(bps => bps.LibraryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<BookPurchaseSuggestion>()
            .HasOne(bps => bps.Processor)
            .WithMany()
            .HasForeignKey(bps => bps.ProcessedByLibrarian)
            .OnDelete(DeleteBehavior.SetNull);
    }

    private void ConfigureIndexes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.CardNumber);

        modelBuilder.Entity<Library>()
            .HasIndex(l => l.Name);

        modelBuilder.Entity<Book>()
            .HasIndex(b => b.LibraryId);

        modelBuilder.Entity<Book>()
            .HasIndex(b => b.ISBN);

        modelBuilder.Entity<BookCopy>()
            .HasIndex(bc => bc.RegistrationNumber)
            .IsUnique();

        modelBuilder.Entity<Loan>()
            .HasIndex(l => l.UserId);

        modelBuilder.Entity<Loan>()
            .HasIndex(l => l.LibraryId);

        modelBuilder.Entity<MagicLinkToken>()
            .HasIndex(mlt => mlt.Token)
            .IsUnique();

        modelBuilder.Entity<OtpToken>()
            .HasIndex(ot => ot.Email);

        modelBuilder.Entity<BookPurchaseSuggestion>()
            .HasIndex(bps => bps.LibraryId);

        modelBuilder.Entity<BookPurchaseSuggestion>()
            .HasIndex(bps => bps.Status);
    }

    private void ConfigureDecimalPrecision(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>()
            .Property(b => b.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Fine>()
            .Property(f => f.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<FinePayment>()
            .Property(fp => fp.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<SystemSettings>()
            .Property(ss => ss.DailyFineRate)
            .HasPrecision(18, 2);

        modelBuilder.Entity<SystemSettings>()
            .Property(ss => ss.LostBookFinePercent)
            .HasPrecision(5, 2);
    }

    private void ConfigureGlobalQueryFilters(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Library>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Role>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<UserRole>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Category>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Language>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Publisher>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<ShelfLocation>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Book>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<BookCopy>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Loan>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Reservation>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Fine>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<FinePayment>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<SystemSettings>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<MagicLinkToken>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<OtpToken>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<BookPurchaseSuggestion>().HasQueryFilter(e => !e.IsDeleted);
    }
}
