using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MTA.Domain.Entities;
using MTA.Infrastructure.Data;
using MTA.Infrastructure.Persistence;
using MTA.Web;
using MTA.Web.Attributes;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "MTA Tennis Academy API",
        Version = "v1",
        Description = "API for online tennis learning platform with coach communication",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "MTA Development Team",
            Email = "dev@mta-tennis.com"
        }
    });
    
    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
    
    // Add XML comments for better documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    c.OperationFilter<FileUploadOperation>();

});

// Configure DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddAutoMapper(typeof(MTA.Application.Mapping.BaseMappingProfile).Assembly);
builder.Services.AddAutoMapper(typeof(MTA.Application.Mapping.AccountMappingProfile).Assembly);
builder.Services.AddAutoMapper(typeof(MTA.Application.Mapping.CourseMappingProfile).Assembly);
builder.Services.AddAutoMapper(typeof(MTA.Application.Mapping.HistoryMappingProfile).Assembly);
builder.Services.AddAutoMapper(typeof(MTA.Application.Mapping.PackageMappingProfile).Assembly);
builder.Services.AddAutoMapper(typeof(MTA.Application.Mapping.SupportMappingProfile).Assembly);


// Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,

            NameClaimType = "UserId",        
            RoleClaimType = ClaimTypes.Role
        };
    });

// Authorization
// 1. Register the custom authorization handler
builder.Services.AddScoped<IAuthorizationHandler, CustomAuthorizationHandler>();

builder.Services.AddValidatorsFromAssemblyContaining<RoleValidator>();

// 2. Register custom policy provider
builder.Services.AddSingleton<IAuthorizationPolicyProvider, CustomAuthorizationPolicyProvider>();



// 3. (Optional) Add default policies if needed
builder.Services.AddAuthorization(options =>
{
    // Example default policy
    options.AddPolicy("RoleAdmin", policy =>
    {
        policy.Requirements.Add(new RoleRequirement("Admin"));
    });

    options.AddPolicy("RoleCoach", policy =>
    {
        policy.Requirements.Add(new RoleRequirement("Coach"));
    });
});

// Add persistence services
builder.Services.AddPersistenceServices(builder.Configuration);

// Add application services
builder.Services.AddApplicationServices();

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

var app = builder.Build();

//// Seed the database
//using (var scope = app.Services.CreateScope())
//{
//    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//    await DbInitializer.SeedAsync(context);
//}

// Configure the HTTP request pipeline.

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MTA API V1");
        // Swagger UI will be available at /swagger
    });


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAll");

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
