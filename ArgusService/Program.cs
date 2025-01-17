using Microsoft.EntityFrameworkCore;
using ArgusService.Data;
using ArgusService.Repositories;
using System;
using ArgusService.Interfaces;
using ArgusService.Managers;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from the .env file
DotNetEnv.Env.Load();

// Retrieve the database connection string from the .env file
string connectionString = Environment.GetEnvironmentVariable("MY_DB_CONNECTION");

// Update the configuration with the connection string
builder.Configuration["ConnectionStrings:MyLocalDbConnection"] = connectionString;

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyLocalDbConnection")));

builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<UserManager>();
builder.Services.AddScoped<TrackerRepository>();
builder.Services.AddScoped<TrackerManager>();
builder.Services.AddScoped<LockRepository>();
builder.Services.AddScoped<NotificationRepository>();
builder.Services.AddScoped<LocationRepository>();
builder.Services.AddScoped<LockManager>();
builder.Services.AddScoped<NotificationManager>();
builder.Services.AddScoped<LocationManager>();
builder.Services.AddScoped<MotionRepository>();
builder.Services.AddScoped<MqttRepository>();
builder.Services.AddScoped<MotionManager>();
builder.Services.AddScoped<MqttManager>();

builder.Services.AddScoped<IUser, UserRepository>();
builder.Services.AddScoped<IUserManager, UserManager>();
builder.Services.AddScoped<ITracker, TrackerRepository>();
builder.Services.AddScoped<ITrackerManager, TrackerManager>();
builder.Services.AddScoped<ILock, LockRepository>();
builder.Services.AddScoped<INotification, NotificationRepository>();
builder.Services.AddScoped<ILocation, LocationRepository>();
builder.Services.AddScoped<ILockManager, LockManager>();
builder.Services.AddScoped<INotificationManager, NotificationManager>();
builder.Services.AddScoped<ILocationManager, LocationManager>();
builder.Services.AddScoped<IMotion, MotionRepository>();
builder.Services.AddScoped<IMqtt, MqttRepository>();
builder.Services.AddScoped<IMotionManager, MotionManager>();
builder.Services.AddScoped<IMqttManager, MqttManager>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\n\nExample: \"Bearer eyJhbGci...\""
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
