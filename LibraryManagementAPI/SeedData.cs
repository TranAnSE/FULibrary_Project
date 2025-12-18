using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FULibraryDbContext>();

        // Ensure database is created
        await context.Database.MigrateAsync();

        // Check if data already exists
        if (await context.Users.AnyAsync())
        {
            return; // Database has been seeded
        }

        Console.WriteLine("Seeding database with initial data...");

        // 1. Create Roles
        var adminRole = new Role
        {
            Id = Guid.NewGuid(),
            Name = "Admin",
            Description = "System Administrator with full access"
        };

        var librarianRole = new Role
        {
            Id = Guid.NewGuid(),
            Name = "Librarian",
            Description = "Library staff managing books and loans"
        };

        var borrowerRole = new Role
        {
            Id = Guid.NewGuid(),
            Name = "Borrower",
            Description = "Library member who can borrow books"
        };

        await context.Roles.AddRangeAsync(adminRole, librarianRole, borrowerRole);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Roles created");

        // 2. Create Libraries
        var hoaLacLibrary = new Library
        {
            Id = Guid.NewGuid(),
            Name = "Thư viện Hòa Lạc",
            Address = "Khu Công nghệ cao Hòa Lạc, Thạch Thất, Hà Nội",
            Email = "library.hoalac@fpt.edu.vn",
            Phone = "(024) 7300 1866",
            WeekdayHours = "7:00 - 22:00",
            WeekendHours = "7:00 - 22:00"
        };

        var hcmLibrary = new Library
        {
            Id = Guid.NewGuid(),
            Name = "Thư viện TP. Hồ Chí Minh",
            Address = "Lô E2a-7, Đường D1, Khu Công nghệ cao, P.Long Thạnh Mỹ, TP.Thủ Đức, TP.HCM",
            Email = "library.hcm@fpt.edu.vn",
            Phone = "(028) 7300 5588",
            WeekdayHours = "7:00 - 22:00",
            WeekendHours = "7:00 - 22:00"
        };

        var danangLibrary = new Library
        {
            Id = Guid.NewGuid(),
            Name = "Thư viện Đà Nẵng",
            Address = "Khu đô thị công nghệ FPT Đà Nẵng, P.Hòa Hải, Q.Ngũ Hành Sơn, TP.Đà Nẵng",
            Email = "library.danang@fpt.edu.vn",
            Phone = "(0236) 730 1111",
            WeekdayHours = "7:00 - 22:00",
            WeekendHours = "7:00 - 22:00"
        };

        await context.Libraries.AddRangeAsync(hoaLacLibrary, hcmLibrary, danangLibrary);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Libraries created");

        // 3. Create System Settings for each library
        var hoaLacSettings = new SystemSettings
        {
            Id = Guid.NewGuid(),
            LibraryId = hoaLacLibrary.Id,
            MaxBooksPerBorrower = 5,
            LoanDurationDays = 14,
            MaxRenewals = 2,
            RenewalDays = 7,
            DailyFineRate = 5000,
            LostBookFinePercent = 200,
            ReservationExpiryDays = 3
        };

        var hcmSettings = new SystemSettings
        {
            Id = Guid.NewGuid(),
            LibraryId = hcmLibrary.Id,
            MaxBooksPerBorrower = 5,
            LoanDurationDays = 14,
            MaxRenewals = 2,
            RenewalDays = 7,
            DailyFineRate = 5000,
            LostBookFinePercent = 200,
            ReservationExpiryDays = 3
        };

        var danangSettings = new SystemSettings
        {
            Id = Guid.NewGuid(),
            LibraryId = danangLibrary.Id,
            MaxBooksPerBorrower = 5,
            LoanDurationDays = 14,
            MaxRenewals = 2,
            RenewalDays = 7,
            DailyFineRate = 5000,
            LostBookFinePercent = 200,
            ReservationExpiryDays = 3
        };

        await context.SystemSettings.AddRangeAsync(hoaLacSettings, hcmSettings, danangSettings);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ System settings created");

        // 4. Create Users
        // BCrypt password hashing for security
        string defaultPasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");

        var adminUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "admin@fpt.edu.vn",
            PasswordHash = defaultPasswordHash,
            FullName = "System Administrator",
            CardNumber = "ADMIN001",
            Phone = "0900000001",
            MustChangePassword = false,
            IsLocked = false,
            HomeLibraryId = hoaLacLibrary.Id
        };

        var librarianUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "librarian@fpt.edu.vn",
            PasswordHash = defaultPasswordHash,
            FullName = "Nguyễn Văn Thư",
            CardNumber = "LIB001",
            Phone = "0900000002",
            MustChangePassword = false,
            IsLocked = false,
            HomeLibraryId = hoaLacLibrary.Id,
            AssignedLibraryId = hoaLacLibrary.Id
        };

        var borrowerUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "student@fpt.edu.vn",
            PasswordHash = defaultPasswordHash,
            FullName = "Trần Thị Sinh Viên",
            CardNumber = "SV001",
            Phone = "0900000003",
            MustChangePassword = false,
            IsLocked = false,
            HomeLibraryId = hoaLacLibrary.Id
        };

        await context.Users.AddRangeAsync(adminUser, librarianUser, borrowerUser);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Users created");

        // 5. Assign Roles to Users
        var userRoles = new List<UserRole>
        {
            new UserRole { Id = Guid.NewGuid(), UserId = adminUser.Id, RoleId = adminRole.Id },
            new UserRole { Id = Guid.NewGuid(), UserId = librarianUser.Id, RoleId = librarianRole.Id },
            new UserRole { Id = Guid.NewGuid(), UserId = borrowerUser.Id, RoleId = borrowerRole.Id }
        };

        await context.UserRoles.AddRangeAsync(userRoles);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ User roles assigned");

        // 6. Create Categories
        var categories = new List<Category>
        {
            new Category { Id = Guid.NewGuid(), Name = "Công nghệ thông tin", Description = "IT, Programming, Software" },
            new Category { Id = Guid.NewGuid(), Name = "Kinh doanh", Description = "Business, Management" },
            new Category { Id = Guid.NewGuid(), Name = "Ngoại ngữ", Description = "English, Japanese" },
            new Category { Id = Guid.NewGuid(), Name = "Khoa học tự nhiên", Description = "Math, Physics, Chemistry" },
            new Category { Id = Guid.NewGuid(), Name = "Văn học", Description = "Literature, Novel" }
        };

        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Categories created");

        // 7. Create Languages
        var languages = new List<Language>
        {
            new Language { Id = Guid.NewGuid(), Name = "Tiếng Việt", Code = "vi" },
            new Language { Id = Guid.NewGuid(), Name = "English", Code = "en" },
            new Language { Id = Guid.NewGuid(), Name = "日本語", Code = "ja" }
        };

        await context.Languages.AddRangeAsync(languages);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Languages created");

        // 8. Create Publishers
        var publishers = new List<Publisher>
        {
            new Publisher { Id = Guid.NewGuid(), Name = "NXB Đại học Quốc gia", Address = "Hà Nội" },
            new Publisher { Id = Guid.NewGuid(), Name = "O'Reilly Media", Address = "USA" },
            new Publisher { Id = Guid.NewGuid(), Name = "Addison-Wesley", Address = "USA" },
            new Publisher { Id = Guid.NewGuid(), Name = "NXB Trẻ", Address = "TP.HCM" }
        };

        await context.Publishers.AddRangeAsync(publishers);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Publishers created");

        // 9. Create Shelf Locations
        var shelfLocations = new List<ShelfLocation>
        {
            new ShelfLocation { Id = Guid.NewGuid(), LibraryId = hoaLacLibrary.Id, Code = "A1", Description = "IT - Floor 1" },
            new ShelfLocation { Id = Guid.NewGuid(), LibraryId = hoaLacLibrary.Id, Code = "A2", Description = "IT - Floor 2" },
            new ShelfLocation { Id = Guid.NewGuid(), LibraryId = hoaLacLibrary.Id, Code = "B1", Description = "Business - Floor 1" },
            new ShelfLocation { Id = Guid.NewGuid(), LibraryId = hcmLibrary.Id, Code = "A1", Description = "IT - Floor 1" },
            new ShelfLocation { Id = Guid.NewGuid(), LibraryId = danangLibrary.Id, Code = "A1", Description = "IT - Floor 1" }
        };

        await context.ShelfLocations.AddRangeAsync(shelfLocations);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Shelf locations created");

        // 10. Create Sample Books
        var books = new List<Book>
        {
            new Book
            {
                Id = Guid.NewGuid(),
                Title = "Clean Code: A Handbook of Agile Software Craftsmanship",
                Author = "Robert C. Martin",
                ISBN = "978-0132350884",
                PublicationYear = 2008,
                DDC = "005.1",
                Subject = "Software Engineering",
                Keyword = "Clean Code, Programming",
                Description = "Even bad code can function. But if code isn't clean, it can bring a development organization to its knees.",
                Price = 450000,
                LibraryId = hoaLacLibrary.Id,
                CategoryId = categories[0].Id,
                LanguageId = languages[1].Id,
                PublisherId = publishers[1].Id
            },
            new Book
            {
                Id = Guid.NewGuid(),
                Title = "Design Patterns: Elements of Reusable Object-Oriented Software",
                Author = "Erich Gamma, Richard Helm, Ralph Johnson, John Vlissides",
                ISBN = "978-0201633610",
                PublicationYear = 1994,
                DDC = "005.1",
                Subject = "Software Design",
                Keyword = "Design Patterns, OOP",
                Description = "Design Patterns is a modern classic in the literature of object-oriented development.",
                Price = 520000,
                LibraryId = hoaLacLibrary.Id,
                CategoryId = categories[0].Id,
                LanguageId = languages[1].Id,
                PublisherId = publishers[2].Id
            },
            new Book
            {
                Id = Guid.NewGuid(),
                Title = "The Pragmatic Programmer",
                Author = "David Thomas, Andrew Hunt",
                ISBN = "978-0135957059",
                PublicationYear = 2019,
                DDC = "005.1",
                Subject = "Programming",
                Keyword = "Best Practices, Software Development",
                Description = "One of the most significant books in my life - Obie Fernandez, Author, The Rails Way",
                Price = 480000,
                LibraryId = hoaLacLibrary.Id,
                CategoryId = categories[0].Id,
                LanguageId = languages[1].Id,
                PublisherId = publishers[2].Id
            }
        };

        await context.Books.AddRangeAsync(books);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Books created");

        // 11. Create Book Copies
        var bookCopies = new List<BookCopy>();
        int copyCounter = 1;

        foreach (var book in books)
        {
            for (int i = 1; i <= 3; i++)
            {
                bookCopies.Add(new BookCopy
                {
                    Id = Guid.NewGuid(),
                    BookId = book.Id,
                    RegistrationNumber = $"DKCB{copyCounter:D6}",
                    Status = BookCopyStatus.Available,
                    ShelfLocationId = shelfLocations[0].Id
                });
                copyCounter++;
            }
        }

        await context.BookCopies.AddRangeAsync(bookCopies);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Book copies created");

        Console.WriteLine();
        Console.WriteLine("🎉 Seed data completed successfully!");
        Console.WriteLine();
        Console.WriteLine("📋 Default Accounts:");
        Console.WriteLine("═══════════════════════════════════════════════════════════");
        Console.WriteLine();
        Console.WriteLine("👤 ADMIN ACCOUNT:");
        Console.WriteLine($"   Email:    admin@fpt.edu.vn");
        Console.WriteLine($"   Password: Admin@123");
        Console.WriteLine($"   Role:     Administrator");
        Console.WriteLine();
        Console.WriteLine("📚 LIBRARIAN ACCOUNT:");
        Console.WriteLine($"   Email:    librarian@fpt.edu.vn");
        Console.WriteLine($"   Password: Admin@123");
        Console.WriteLine($"   Role:     Librarian (Hòa Lạc Library)");
        Console.WriteLine();
        Console.WriteLine("👨‍🎓 BORROWER ACCOUNT:");
        Console.WriteLine($"   Email:    student@fpt.edu.vn");
        Console.WriteLine($"   Password: Admin@123");
        Console.WriteLine($"   Role:     Borrower");
        Console.WriteLine();
        Console.WriteLine("═══════════════════════════════════════════════════════════");
        Console.WriteLine();
        Console.WriteLine("📊 Data Summary:");
        Console.WriteLine($"   • 3 Libraries (Hòa Lạc, HCM, Đà Nẵng)");
        Console.WriteLine($"   • 3 Users (Admin, Librarian, Borrower)");
        Console.WriteLine($"   • 3 Roles (Admin, Librarian, Borrower)");
        Console.WriteLine($"   • 5 Categories");
        Console.WriteLine($"   • 3 Languages");
        Console.WriteLine($"   • 4 Publishers");
        Console.WriteLine($"   • 3 Books with 9 copies");
        Console.WriteLine();
    }
}
