using Microsoft.EntityFrameworkCore;
using ArgusService.Data;
using ArgusService.Repositories;
using System;
using ArgusService.Interfaces;
using ArgusService.Managers;

var builder = WebApplication.CreateBuilder(args);

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

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
