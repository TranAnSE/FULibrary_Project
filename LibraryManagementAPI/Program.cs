using System.Text;
using System.Text.Json.Serialization;
using BusinessObjects;
using DataAccessObjects;
using LibraryManagementAPI.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using Repositories;
using Services;

var builder = WebApplication.CreateBuilder(args);

// Configure JSON options
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

// Add OData
var modelBuilder = new ODataConventionModelBuilder();
modelBuilder.EntitySet<User>("Users");
modelBuilder.EntitySet<Library>("Libraries");
modelBuilder.EntitySet<Book>("Books");
modelBuilder.EntitySet<BookCopy>("BookCopies");
modelBuilder.EntitySet<Loan>("Loans");
modelBuilder.EntitySet<Reservation>("Reservations");
modelBuilder.EntitySet<Fine>("Fines");
modelBuilder.EntitySet<Category>("Categories");
modelBuilder.EntitySet<Language>("Languages");
modelBuilder.EntitySet<Publisher>("Publishers");
modelBuilder.EntitySet<ShelfLocation>("ShelfLocations");

// Configure JSON options for controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    })
    .AddOData(options => options
        .Select()
        .Filter()
        .OrderBy()
        .Expand()
        .Count()
        .SetMaxTop(100)
        .AddRouteComponents("odata", modelBuilder.GetEdmModel()));

// Add DbContext
builder.Services.AddDbContext<FULibraryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")));

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Register DAOs
builder.Services.AddScoped(typeof(IBaseDAO<>), typeof(BaseDAO<>));
builder.Services.AddScoped<IUserDAO, UserDAO>();
builder.Services.AddScoped<ILibraryDAO, LibraryDAO>();
builder.Services.AddScoped<IBookDAO, BookDAO>();
builder.Services.AddScoped<IBookCopyDAO, BookCopyDAO>();
builder.Services.AddScoped<ILoanDAO, LoanDAO>();
builder.Services.AddScoped<IReservationDAO, ReservationDAO>();
builder.Services.AddScoped<IFineDAO, FineDAO>();
builder.Services.AddScoped<ISystemSettingsDAO, SystemSettingsDAO>();
builder.Services.AddScoped<IMagicLinkTokenDAO, MagicLinkTokenDAO>();
builder.Services.AddScoped<IOtpTokenDAO, OtpTokenDAO>();
builder.Services.AddScoped<ICategoryDAO, CategoryDAO>();
builder.Services.AddScoped<ILanguageDAO, LanguageDAO>();
builder.Services.AddScoped<IPublisherDAO, PublisherDAO>();
builder.Services.AddScoped<IShelfLocationDAO, ShelfLocationDAO>();
builder.Services.AddScoped<IBookPurchaseSuggestionDAO, BookPurchaseSuggestionDAO>();

// Register Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ILibraryRepository, LibraryRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<ILoanRepository, LoanRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IFineRepository, FineRepository>();
builder.Services.AddScoped<ISystemSettingsRepository, SystemSettingsRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ILanguageRepository, LanguageRepository>();
builder.Services.AddScoped<IPublisherRepository, PublisherRepository>();
builder.Services.AddScoped<IShelfLocationRepository, ShelfLocationRepository>();
builder.Services.AddScoped<IBookSuggestionRepository, BookSuggestionRepository>();

// Register Services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILibraryService, LibraryService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<ILoanService, LoanService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IFineService, FineService>();
builder.Services.AddScoped<IEmailService, GmailEmailService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped(typeof(ICatalogService<>), typeof(CatalogService<>));

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FU Library Management API",
        Version = "v1",
        Description = "API for managing library operations with JWT authentication"
    });

    // Add JWT Bearer authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.\n\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "DefaultSecretKeyForDevelopmentOnlyChangeInProduction123456";
var issuer = jwtSettings["Issuer"] ?? "FULibrary";
var audience = jwtSettings["Audience"] ?? "FULibraryUsers";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Add Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("Librarian", policy => policy.RequireRole("Librarian", "Admin"));
    options.AddPolicy("Borrower", policy => policy.RequireRole("Borrower", "Librarian", "Admin"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseLibraryScope();
app.UseAuthorization();

app.MapControllers();

// Seed initial data
await LibraryManagementAPI.SeedData.Initialize(app.Services);

app.Run();
