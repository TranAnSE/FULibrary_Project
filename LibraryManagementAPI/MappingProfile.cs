using AutoMapper;
using BusinessObjects;
using LibraryManagementAPI.DTOs.Auth;
using LibraryManagementAPI.DTOs.Books;
using LibraryManagementAPI.DTOs.Catalogs;
using LibraryManagementAPI.DTOs.Fines;
using LibraryManagementAPI.DTOs.Libraries;
using LibraryManagementAPI.DTOs.Loans;
using LibraryManagementAPI.DTOs.Reservations;
using LibraryManagementAPI.DTOs.Users;
using LibraryManagementAPI.DTOs.SystemSettings;

namespace LibraryManagementAPI;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.HomeLibraryName, opt => opt.MapFrom(src => src.HomeLibrary != null ? src.HomeLibrary.Name : null))
            .ForMember(dest => dest.AssignedLibraryName, opt => opt.MapFrom(src => src.AssignedLibrary != null ? src.AssignedLibrary.Name : null))
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.Name).ToList()));

        CreateMap<CreateUserDto, User>();
        CreateMap<UpdateUserDto, User>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Library mappings
        CreateMap<Library, LibraryDto>();
        CreateMap<CreateLibraryDto, Library>();

        // Book mappings
        CreateMap<Book, BookDto>()
            .ForMember(dest => dest.LibraryName, opt => opt.MapFrom(src => src.Library.Name))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
            .ForMember(dest => dest.LanguageName, opt => opt.MapFrom(src => src.Language != null ? src.Language.Name : null))
            .ForMember(dest => dest.PublisherName, opt => opt.MapFrom(src => src.Publisher != null ? src.Publisher.Name : null))
            .ForMember(dest => dest.TotalCopies, opt => opt.MapFrom(src => src.Copies.Count))
            .ForMember(dest => dest.AvailableCopies, opt => opt.MapFrom(src => src.Copies.Count(c => c.Status == BookCopyStatus.Available)));

        CreateMap<Book, BookDetailDto>()
            .ForMember(dest => dest.LibraryName, opt => opt.MapFrom(src => src.Library.Name))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
            .ForMember(dest => dest.LanguageName, opt => opt.MapFrom(src => src.Language != null ? src.Language.Name : null))
            .ForMember(dest => dest.PublisherName, opt => opt.MapFrom(src => src.Publisher != null ? src.Publisher.Name : null))
            .ForMember(dest => dest.Copies, opt => opt.MapFrom(src => src.Copies));

        CreateMap<BookCopy, BookCopyDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.ShelfLocationCode, opt => opt.MapFrom(src => src.ShelfLocation != null ? src.ShelfLocation.Code : null));

        CreateMap<CreateBookDto, Book>();
        CreateMap<UpdateBookDto, Book>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<CreateBookCopyDto, BookCopy>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => BookCopyStatus.Available));

        // Loan mappings
        CreateMap<Loan, LoanDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName))
            .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.RegistrationNumber, opt => opt.MapFrom(src => src.BookCopy.RegistrationNumber))
            .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.BookCopy.Book.Title))
            .ForMember(dest => dest.BookCoverImageUrl, opt => opt.MapFrom(src => src.BookCopy.Book.CoverImageUrl))
            .ForMember(dest => dest.LibraryName, opt => opt.MapFrom(src => src.Library != null ? src.Library.Name : string.Empty))
            .ForMember(dest => dest.OverdueDays, opt => opt.MapFrom(src => src.ReturnDate == null && src.DueDate < DateTime.UtcNow ? (DateTime.UtcNow.Date - src.DueDate.Date).Days : 0))
            .ForMember(dest => dest.RenewalsRemaining, opt => opt.Ignore());

        CreateMap<CreateLoanDto, Loan>();

        // Reservation mappings
        CreateMap<Reservation, ReservationDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName))
            .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
            .ForMember(dest => dest.BookCoverImageUrl, opt => opt.MapFrom(src => src.Book.CoverImageUrl))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<CreateReservationDto, Reservation>();

        // Fine mappings
        CreateMap<Fine, FineDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName))
            .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Loan != null && src.Loan.BookCopy != null ? src.Loan.BookCopy.Book.Title : null))
            .ForMember(dest => dest.TotalPaid, opt => opt.MapFrom(src => src.Payments.Sum(p => p.Amount)))
            .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Amount - src.Payments.Sum(p => p.Amount)))
            .ForMember(dest => dest.WaivedByName, opt => opt.Ignore());

        CreateMap<FinePayment, FinePaymentDto>()
            .ForMember(dest => dest.ReceivedByName, opt => opt.Ignore());

        // SystemSettings mappings
        CreateMap<SystemSettings, SystemSettingsDto>()
            .ForMember(dest => dest.LibraryName, opt => opt.MapFrom(src => src.Library.Name));
        CreateMap<UpdateSystemSettingsDto, SystemSettings>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Catalog mappings
        CreateMap<Category, CategoryDto>();
        CreateMap<CreateCategoryDto, Category>();

        CreateMap<Language, LanguageDto>();
        CreateMap<CreateLanguageDto, Language>();

        CreateMap<Publisher, PublisherDto>();
        CreateMap<CreatePublisherDto, Publisher>();

        CreateMap<ShelfLocation, ShelfLocationDto>()
            .ForMember(dest => dest.LibraryName, opt => opt.MapFrom(src => src.Library.Name));
        CreateMap<CreateShelfLocationDto, ShelfLocation>();
    }
}
