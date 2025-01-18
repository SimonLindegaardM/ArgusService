// File: Program.cs

using ArgusService.Interfaces;
using ArgusService.Managers;
using ArgusService.Models;
using ArgusService.Repositories;
using ArgusService.Data;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MQTTnet;
using NLog;
using NLog.Web;
using System.Reflection;
using System.Text;

var logger = NLog.LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
try
{
    logger.Debug("Init main");

    var builder = WebApplication.CreateBuilder(args);

    // ---------------------------------------------
    // 1. Load Environment Variables from .env File
    // ---------------------------------------------

    // Ensure the .env file is loaded before accessing any environment variables
    Env.Load();

    // ---------------------------------------------
    // 2. Configure Logging
    // ---------------------------------------------

    // Clear default logging providers
    builder.Logging.ClearProviders();

    // Use NLog as the logging provider
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
    builder.Host.UseNLog();

    // ---------------------------------------------
    // 3. Register Configuration Settings
    // ---------------------------------------------

    // Bind MQTT settings from environment variables to the MqttSettings class
    builder.Services.Configure<MqttSettings>(options =>
    {
        options.ClientId = Environment.GetEnvironmentVariable("MQTT_CLIENT_ID") ?? "DefaultClientId";
        options.Broker = Environment.GetEnvironmentVariable("MQTT_BROKER") ?? "broker.example.com";
        options.Port = int.TryParse(Environment.GetEnvironmentVariable("MQTT_PORT"), out var port) ? port : 1883;
        options.CleanSession = bool.TryParse(Environment.GetEnvironmentVariable("MQTT_CLEAN_SESSION"), out var cleanSession) ? cleanSession : true;
        options.Username = Environment.GetEnvironmentVariable("MQTT_USERNAME"); // Optional
        options.Password = Environment.GetEnvironmentVariable("MQTT_PASSWORD"); // Optional
    });

    // ---------------------------------------------
    // 4. Register Services with Dependency Injection (DI)
    // ---------------------------------------------

    // 4.1. Register MQTT Client as Singleton
    builder.Services.AddSingleton<IMqttClient>(sp =>
    {
        var factory = new MqttClientFactory();
        return factory.CreateMqttClient();
    });

    // 4.2. Register Repositories
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<ITrackerRepository, TrackerRepository>();
    builder.Services.AddScoped<ILockRepository, LockRepository>();
    builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
    builder.Services.AddScoped<ILocationRepository, LocationRepository>();
    builder.Services.AddScoped<IMotionRepository, MotionRepository>();
    builder.Services.AddScoped<IMqttRepository, MqttRepository>();

    // 4.3. Register Managers
    builder.Services.AddScoped<IUserManager, UserManager>();
    builder.Services.AddScoped<ITrackerManager, TrackerManager>();
    builder.Services.AddScoped<ILockManager, LockManager>();
    builder.Services.AddScoped<INotificationManager, NotificationManager>();
    builder.Services.AddScoped<ILocationManager, LocationManager>();
    builder.Services.AddScoped<IMotionManager, MotionManager>();
    builder.Services.AddScoped<IMqttManager, MqttManager>();

    // ---------------------------------------------
    // 5. Register DbContext (Entity Framework Core)
    // ---------------------------------------------

    var dbConnectionString = Environment.GetEnvironmentVariable("MY_DB_CONNECTION")
                              ?? builder.Configuration.GetConnectionString("MyLocalDbConnection");

    if (string.IsNullOrEmpty(dbConnectionString))
    {
        throw new InvalidOperationException("Database connection string 'MY_DB_CONNECTION' is not set.");
    }

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(dbConnectionString));

    // ---------------------------------------------
    // 6. Configure Authentication and Authorization
    // ---------------------------------------------

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        // Retrieve JWT settings from environment variables
        var jwtIssuer = Environment.GetEnvironmentVariable("Jwt_Issuer") ?? "DefaultIssuer";
        var jwtAudience = Environment.GetEnvironmentVariable("Jwt_Audience") ?? "DefaultAudience";
        var jwtKey = Environment.GetEnvironmentVariable("Jwt_Key") ?? "DefaultSecretKey12345";

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

    builder.Services.AddAuthorization();

    // ---------------------------------------------
    // 7. Register Controllers
    // ---------------------------------------------

    // IMPORTANT: Registers MVC controllers with the DI container.
    // Essential for routing HTTP requests to controller actions.
    builder.Services.AddControllers();

    // ---------------------------------------------
    // 8. Configure Swagger for API Documentation
    // ---------------------------------------------

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "ArgusService API", Version = "v1" });

        // Include XML comments if enabled in .csproj for better documentation
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (System.IO.File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }

        // Configure JWT Authentication in Swagger
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter 'Bearer' [space] and then your valid token."
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
                Array.Empty<string>()
            }
        });
    });

    // ---------------------------------------------
    // 9. Configure CORS (Cross-Origin Resource Sharing)
    // ---------------------------------------------

    // OPTIONAL: Adjust the allowed origins as per your requirements
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy", policyBuilder =>
        {
            policyBuilder.WithOrigins("https://yourdomain.com") // Replace with your client domain
                         .AllowAnyMethod()
                         .AllowAnyHeader();
        });
    });

    // ---------------------------------------------
    // 10. Build the Application
    // ---------------------------------------------

    var app = builder.Build();

    // ---------------------------------------------
    // 11. Configure the HTTP Request Pipeline
    // ---------------------------------------------

    // 11.1. Use Swagger in Development Environment
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage(); // Enable detailed error pages in development
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error"); // Generic error page for production
        app.UseHsts();
    }

    // 11.2. Use CORS Policy
    app.UseCors("CorsPolicy");

    // 11.3. Enforce HTTPS Redirection
    app.UseHttpsRedirection();

    // 11.4. Enable Authentication and Authorization Middleware
    app.UseAuthentication();
    app.UseAuthorization();

    // 11.5. Map Controller Routes
    app.MapControllers();

    // ---------------------------------------------
    // 12. Initialize MQTT Connection at Startup
    // ---------------------------------------------

    // Execute asynchronous tasks before the application starts handling requests
    using (var scope = app.Services.CreateScope())
    {
        var mqttManager = scope.ServiceProvider.GetRequiredService<IMqttManager>();

        try
        {
            // Initialize MQTT connection
            await mqttManager.InitializeConnectionAsync();
            logger.Info("MQTT connection initialized successfully.");

            // Start monitoring MQTT connection status
            await mqttManager.MonitorConnectionStatusAsync();
            logger.Info("MQTT connection monitoring started.");
        }
        catch (Exception ex)
        {
            // Log the exception and decide whether to terminate the application
            logger.Error(ex, "Failed to initialize MQTT connection.");
            throw;
        }
    }

    // ---------------------------------------------
    // 13. Run the Application
    // ---------------------------------------------

    app.Run();
}
catch (Exception ex)
{
    // NLog: catch setup errors
    logger.Error(ex, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}
