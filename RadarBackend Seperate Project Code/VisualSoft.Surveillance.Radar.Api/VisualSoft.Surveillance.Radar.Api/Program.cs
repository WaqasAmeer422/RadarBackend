using EasyNetQ;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using System.Text.Json;
using VisualSoft.Surveillance.Radar.Api.Infrastructure;
using VisualSoft.Surveillance.Radar.Api.Middlewares;
using VisualSoft.Surveillance.Radar.Api.Profiles;
using VisualSoft.Surveillance.Radar.Data.Infrastructure;
using VisualSoft.Surveillance.Radar.Domain;
using VisualSoft.Surveillance.Radar.Domain.Configurations;
using VisualSoft.Surveillance.Radar.Domain.Models;
using VisualSoft.Surveillance.Radar.Api.Infrastructure;
using VisualSoft.Surveillance.Radar.Api.Middlewares;
using VisualSoft.Surveillance.Radar.Api.Profiles;
using VisualSoft.Surveillance.Radar.Data.Infrastructure;
using VisualSoft.Surveillance.Radar.Domain.Configurations;
using VisualSoft.Surveillance.Radar.Domain.Models;
using VisualSoft.Surveillance.Radar.Services.Adapters;
using VisualSoft.Surveillance.Radar.Services.Handlers;
using VisualSoft.Surveillance.Radar.Services.HostedServices;
using VisualSoft.Surveillance.Radar.Services.Infrastructure;
using VisualSoft.Surveillance.Radar.Services.RabbitMQ;
using VisualSoft.Surveillance.Radar.Services;
using VisualSoft.Surveillance.Radar.Services;
using VisualSoft.Surveillance.Radar.Services.Adapters;
using VisualSoft.Surveillance.Radar.Services.Handlers;
using VisualSoft.Surveillance.Radar.Services.HostedServices;
using VisualSoft.Surveillance.Radar.Services.Infrastructure;
using VisualSoft.Surveillance.Radar.Services.RabbitMQ;
using ConnectionFactory = VisualSoft.Surveillance.Radar.Data.Infrastructure.ConnectionFactory;
using IConnectionFactory = VisualSoft.Surveillance.Radar.Data.Infrastructure.IConnectionFactory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<RabbitMQConfiguration>(builder.Configuration.GetSection("RabbitMQConfiguration"));
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddHttpClient("TokenClient", client =>
{
    // The BaseAddress will be set inside the ServiceUserAuthTokenExtractor
    // The factory will manage the HttpClient's lifecycle
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("X-CallerUrl", builder.Configuration["ApiLinkUrl:BaseUrl"]);
});
builder.Services.AddHttpClient("AuthenticatedClient", client =>
{
    // Configure default headers if needed
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});
builder.Services.AddScoped<IOrganisationAdapter, OrganisationAdapter>();
builder.Services.AddSingleton<IServiceConfiguration, ServiceConfiguration>();
builder.Services.AddScoped<IRabbitMQEventService, RabbitMQEventService>();
builder.Services.AddHostedService<ParserRequestRadarConsumerService>(); 
builder.Services.AddScoped<IRadarTransactionService, RadarTransactionService>(); 
builder.Services.AddScoped<ITransactionLogServiceHandler, TransactionServiceHandler>();
builder.Services.AddTransient<IAuthTokenExtractor, ServiceUserAuthTokenExtractor>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IConnectionFactory, ConnectionFactory>();
builder.Services.AddScoped<IQueryExecuter, QueryExecuter>();
builder.Services.AddSingleton<IDBMigrator, DBMigrator>();
builder.Services.AddSingleton<IMemoryCache, MemoryCache>();

// Configure EasyNetQ with Serilog here ---
builder.Services.AddSingleton<IBus>(sp =>
{
    var rabbitMqConfig = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<RabbitMQConfiguration>>().Value;

    return RabbitHutch.CreateBus(
        rabbitMqConfig.ConnectionString);
});

ConfigureLogger(builder);


builder.Services.AddControllers(options =>
{
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

var allowedOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();
var allowSpecificOrigins = "allowSpecificOrigins"; // Define a name for your CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowSpecificOrigins,
                      builder =>
                      {
                          // Use the allowedOrigins array from configuration
                          if (allowedOrigins != null)
                          {
                              builder.WithOrigins(allowedOrigins)
                                    .AllowAnyHeader()
                                    .AllowAnyMethod()
                                    .AllowCredentials()
                                    .WithExposedHeaders("X-Pagination");
                          }
                      });
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

ConfigureTokenAuthorization(builder);
ConfigurePermissions(builder);

ConfigureHealthCheck(builder);

var app = builder.Build();
var env = app.Services.GetRequiredService<IWebHostEnvironment>();


app.Services.GetRequiredService<IDBMigrator>().DBMigrate();
ConfigureDbMigration(builder);

if (env.IsDevelopment())
{
    ConfigureSwagger(builder);
}

app.UseHttpsRedirection();

app.UseExceptionMiddleware();

app.UseCors(allowSpecificOrigins);


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.MapHealthChecks("/health");

app.MapHealthChecks("/health/details", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/db", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("db"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecks("/health/managementapi", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("managementapi"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});


app.Run();

void ConfigureHealthCheck(WebApplicationBuilder builder)
{
    builder.Services.AddHealthChecks()
   .AddNpgSql(
        connectionString: builder.Configuration.GetValue<string?>("Connections:PostgresConnectionString"),
        healthQuery: "SELECT 1;",
        name: "PostgreSQL Database",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "db", "sql", "postgres" }
    )
    .AddUrlGroup(
        new Uri(builder.Configuration.GetValue<string?>("ApiLinkUrl:VisualSoft.Surveillance.Management.Api") + "health"),
        name: "AuthenticationApiHealthCheck",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "managementapi" }
    );
}

void ConfigureSwagger(WebApplicationBuilder builder)
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DefaultModelsExpandDepth(-1);
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "VisualSoft Surveillance API v1");
    });
}

void ConfigureLogger(WebApplicationBuilder builder)
{
    Log.Logger = new LoggerConfiguration()
      .ReadFrom.Configuration(builder.Configuration)
      .Enrich.FromLogContext()

      .CreateLogger();

    builder.Logging.ClearProviders();
    builder.Services.AddSingleton(Log.Logger);
    builder.Host.UseSerilog();
}


void ConfigureDbMigration(WebApplicationBuilder builder)
{
    var sp = builder.Services.BuildServiceProvider();

    var dbmigrator = sp.GetService<IDBMigrator>();
    dbmigrator.DBMigrate();
}

void ConfigureTokenAuthorization(WebApplicationBuilder builder)
{
    var authConfiguration = new TokenConfiguration(builder.Configuration);
    builder.Services.AddSingleton<ITokenConfiguration>(authConfiguration);
    builder.Services.AddScoped<CurrentUserProvider>();
    builder.Services.AddScoped<IUserIdentificationModel>(s => s.GetService<CurrentUserProvider>().CurrentUser);

    builder.Services.AddAuthentication((JwtBearerDefaults.AuthenticationScheme)).AddJwtBearer(option =>
    {
        option.UseSecurityTokenValidators = true;
        option.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true, // TODO why true does not work
            ValidIssuer = authConfiguration.Issuer,
            ValidateAudience = true,
            ValidAudience = authConfiguration.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authConfiguration.Key))
        };
        option.Events = new JwtBearerEvents
        {
            OnMessageReceived = context => OnMessageReceived(context),
            OnTokenValidated = context => OnJwtTokenValidated(context),
            OnAuthenticationFailed = context =>
            {
                // Log the exception details here. This will tell you exactly why the token was rejected.
                Log.Error(context.Exception, "JWT Authentication failed: {ErrorMessage}", context.Exception.Message);
                return Task.CompletedTask;
            }
        };
    });
}

void ConfigurePermissions(WebApplicationBuilder builder)
{
    var authorizationPolicyBuilder = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .Build();

    builder.Services.AddAuthorization(options =>
    {
        options.DefaultPolicy = authorizationPolicyBuilder;
        options.InvokeHandlersAfterFailure = false;

        foreach (var field in typeof(Constants.Permissions).GetFields())
        {
            var val = field.GetValue(null).ToString();
            options.AddPolicy(val, policy => policy.AddRequirements(new PermissionRequirement(val)));
        }
    });
    builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
}

Task OnJwtTokenValidated(TokenValidatedContext context)
{
    var currentUserProvider =
                context.HttpContext.RequestServices.GetService<CurrentUserProvider>();

    currentUserProvider.BuildUp(context);

    return Task.CompletedTask;

}

Task OnMessageReceived(MessageReceivedContext context)
{
    string authorization = context.Request.Headers["Authorization"];
    if (string.IsNullOrEmpty(authorization))
    {
        context.Token = null;
    }
    else if (authorization.StartsWith(Constants.JwtToken.BEARER_PREFIX, StringComparison.OrdinalIgnoreCase))
    {
        context.Token = authorization.Substring(Constants.JwtToken.BEARER_PREFIX.Length).Trim();
    }

    return Task.CompletedTask;
}
