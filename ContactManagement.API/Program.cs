using ContactManagement.BLL.Abstract;
using ContactManagement.BLL.Concrete;
using ContactManagement.DAL;
using ContactManagement.DAL.Abstract;
using ContactManagement.DAL.Concrete;
using ContactManagement.DAL.Singletons;
using Microsoft.EntityFrameworkCore;
using FluentValidation.AspNetCore;
using ContactManagement.BLL.Validators;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ContactManagement.BLL.Handlers;
using ContactManagement.BLL.PipelineBehaviors;
using Microsoft.OpenApi.Models;
using ContactManagement.Models.Entities;
using ContactManagement.DAL.Configuration;


var builder = WebApplication.CreateBuilder(args);

// Connection string for the database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//Fluent Validators
builder.Services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CountryValidator>());
builder.Services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CompanyValidator>());

//MediaTR
builder.Services.AddMediatR(typeof(GetAllCountriesHandler).Assembly); // Registers handlers from a specific assembly

//Pipeline Behavior
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

// Register the factory
builder.Services.AddTransient<IRepositoryFactory, RepositoryFactory>();

// Register generic repositories
builder.Services.AddTransient<IRepository<Company>, CompanyRepository>();
builder.Services.AddTransient<IRepository<Contact>, ContactRepository>();
builder.Services.AddTransient<IRepository<Country>, CountryRepository>();

// Register specific repositories
builder.Services.AddTransient<ICompanyRepository, CompanyRepository>();
builder.Services.AddTransient<IContactRepository, ContactRepository>();
builder.Services.AddTransient<ICountryRepository, CountryRepository>();

// Register services
builder.Services.AddTransient<ICompanyService, CompanyService>();
builder.Services.AddTransient<ICountryService, CountryService>();
builder.Services.AddTransient<IContactService, ContactService>();

// Register DatabaseSettings
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("ConnectionStrings"));

// Add controllers and NewtonsoftJson for handling circular references
builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

// Add Swagger generation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "YourIssuer",  
            ValidAudience = "YourAudience",  
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("my32byteverysecretkey12345678901"))  
        };
    });





// Add controllers
builder.Services.AddControllers();

// Add Swagger for API documentation+
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Please enter 'Bearer' followed by a space and your JWT token"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});





var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Enable Swagger middleware in the pipeline
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ContactManagement API v1");
    options.RoutePrefix = string.Empty; // Makes Swagger UI available at the root ("/")
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Apply migrations automatically
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        LoggerSingleton.Instance.Log("Checking for pending migrations...");
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        LoggerSingleton.Instance.Log($"Error applying migrations: {ex.Message}");
    }
}

LoggerSingleton.Instance.Log("Application is starting...!");

//MinimalAPI
app.MapGet("api/v1/countries/minimalAPI", async (ICountryService countryService) =>
{
    var countries = await countryService.GetAllAsync();
    return Results.Ok(countries);
})
.WithName("GetAllCountriesMinAPI")
.WithTags("Countries");




app.Run();
