using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MTA.Application.Services;
using MTA.Domain.Entities;
using MTA.Infrastructure.Persistence;
using MTA.Web;
using MTA.Web.Attributes;
using MTA.Web.Hubs;
using MTA.Web.Middleware;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();
builder.Services.AddSignalR();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

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

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
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

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);

    c.OperationFilter<FileUploadOperation>();
});

builder.Services.AddAutoMapper(typeof(MTA.Application.Mapping.BaseMappingProfile).Assembly);

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

        // Allow SignalR to receive JWT via access_token query string during WebSocket handshake
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddScoped<IAuthorizationHandler, CustomAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, CustomAuthorizationPolicyProvider>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RoleAdmin", policy => policy.Requirements.Add(new RoleRequirement("Admin")));
    options.AddPolicy("RoleCoach", policy => policy.Requirements.Add(new RoleRequirement("Coach")));
});

builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddApplicationServices();

// SignalR chat notifier (bridges Application layer to SignalR hub)
builder.Services.AddScoped<IChatNotifier, ChatNotifier>();

var allowedCorsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendWithCredentials", policy =>
    {
        if (allowedCorsOrigins.Length > 0)
            policy.WithOrigins(allowedCorsOrigins).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
        else
            throw new InvalidOperationException("Cors:AllowedOrigins must be configured in appsettings. The API will not start with a wildcard CORS policy.");
    });
});

var app = builder.Build();

// Must be first so it wraps every downstream middleware/controller call.
app.UseGlobalExceptionHandling();

// Trust reverse-proxy headers (Nginx sets X-Forwarded-For / X-Forwarded-Proto)
app.UseForwardedHeaders();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MTA.Infrastructure.Data.ApplicationDbContext>();
    db.Database.Migrate();
    await MTA.Infrastructure.Data.DbInitializer.SeedAsync(db);
}

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MTA API V1"));

app.UseStaticFiles();
app.UseRouting();

app.UseCors("FrontendWithCredentials");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");

app.Run();
