// File: ArgusService/MappingProfile.cs

using AutoMapper;
using ArgusService.DTOs;
using ArgusService.Models;
using System;

namespace ArgusService
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ---------------------------------------------
            // 1. RegisterTrackerRequestDto -> Tracker
            // ---------------------------------------------
            CreateMap<RegisterTrackerRequestDto, Tracker>()
                .ForMember(dest => dest.TrackerId, opt => opt.MapFrom(src => src.TrackerId))
                
                .ForMember(dest => dest.MqttUsername, opt => opt.MapFrom(src => src.MqttUsername))
               // .ForMember(dest => dest.MqttPassword, opt => opt.MapFrom(src => src.MqttPassword))
               // .ForMember(dest => dest.Psk, opt => opt.MapFrom(src => src.Psk))
                .ForMember(dest => dest.BrokerUrl, opt => opt.MapFrom(src => src.BrokerUrl))
                .ForMember(dest => dest.Port, opt => opt.MapFrom(src => src.Port))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))

                // Explicitly ignore unmapped properties
                .ForMember(dest => dest.FirebaseUID, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.LockState, opt => opt.Ignore())
                .ForMember(dest => dest.DesiredLockState, opt => opt.Ignore())
                .ForMember(dest => dest.LastKnownLocation, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdated, opt => opt.Ignore())
                .ForMember(dest => dest.Locks, opt => opt.Ignore())
                .ForMember(dest => dest.Motions, opt => opt.Ignore())
                .ForMember(dest => dest.MQTTs, opt => opt.Ignore())
                .ForMember(dest => dest.Locations, opt => opt.Ignore());

            // ---------------------------------------------
            // 2. LinkDeviceRequestDto -> Tracker
            // ---------------------------------------------
            CreateMap<LinkDeviceRequestDto, Tracker>()
                .ForMember(dest => dest.TrackerId, opt => opt.MapFrom(src => src.TrackerId))
                .ForMember(dest => dest.FirebaseUID, opt => opt.MapFrom(src => src.FirebaseUID))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))

                // Explicitly ignore unmapped properties
            
                .ForMember(dest => dest.MqttUsername, opt => opt.Ignore())
                .ForMember(dest => dest.MqttPassword, opt => opt.Ignore())
                //.ForMember(dest => dest.Psk, opt => opt.Ignore())
                .ForMember(dest => dest.BrokerUrl, opt => opt.Ignore())
                .ForMember(dest => dest.Port, opt => opt.Ignore())
                .ForMember(dest => dest.LockState, opt => opt.Ignore())
                .ForMember(dest => dest.DesiredLockState, opt => opt.Ignore())
                .ForMember(dest => dest.LastKnownLocation, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdated, opt => opt.Ignore())
                .ForMember(dest => dest.Locks, opt => opt.Ignore())
                .ForMember(dest => dest.Motions, opt => opt.Ignore())
                .ForMember(dest => dest.MQTTs, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Locations, opt => opt.Ignore());

            // ---------------------------------------------
            // 3. UpdateTrackerLockStateRequestDto -> Tracker
            // ---------------------------------------------
            CreateMap<UpdateTrackerLockStateRequestDto, Tracker>()
                .ForMember(dest => dest.DesiredLockState, opt => opt.MapFrom(src => src.DesiredLockState))

                // Explicitly ignore unmapped properties
                .ForMember(dest => dest.TrackerId, opt => opt.Ignore())
              
                .ForMember(dest => dest.MqttUsername, opt => opt.Ignore())
                .ForMember(dest => dest.MqttPassword, opt => opt.Ignore())
               // .ForMember(dest => dest.Psk, opt => opt.Ignore())
                .ForMember(dest => dest.BrokerUrl, opt => opt.Ignore())
                .ForMember(dest => dest.Port, opt => opt.Ignore())
                .ForMember(dest => dest.LockState, opt => opt.Ignore())
                .ForMember(dest => dest.LastKnownLocation, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdated, opt => opt.Ignore())
                .ForMember(dest => dest.Locks, opt => opt.Ignore())
                .ForMember(dest => dest.Motions, opt => opt.Ignore())
                .ForMember(dest => dest.MQTTs, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Locations, opt => opt.Ignore());

            // ---------------------------------------------
            // 4. RegisterLockRequestDto -> Lock
            // ---------------------------------------------
            CreateMap<RegisterLockRequestDto, Lock>()
                .ForMember(dest => dest.LockId, opt => opt.MapFrom(src => src.LockId))
                .ForMember(dest => dest.TrackerId, opt => opt.MapFrom(src => src.TrackerId))

                // Explicitly ignore unmapped properties
                .ForMember(dest => dest.FirebaseUID, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdated, opt => opt.Ignore())
                .ForMember(dest => dest.Tracker, opt => opt.Ignore());

            // ---------------------------------------------
            // 5. UpdateLockLockStateRequestDto -> Lock
            // ---------------------------------------------
            CreateMap<UpdateLockLockStateRequestDto, Lock>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.LockState))

                // Explicitly ignore unmapped properties
                .ForMember(dest => dest.LockId, opt => opt.Ignore())
                .ForMember(dest => dest.TrackerId, opt => opt.Ignore())
                .ForMember(dest => dest.FirebaseUID, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdated, opt => opt.Ignore())
                .ForMember(dest => dest.Tracker, opt => opt.Ignore());

            // ---------------------------------------------
            // 6. MqttPublishRequestDto -> MqttMessage
            // ---------------------------------------------
            CreateMap<MqttPublishRequestDto, MqttMessage>()
                .ForMember(dest => dest.TrackerId, opt => opt.MapFrom(src => src.TrackerId))
                .ForMember(dest => dest.TopicType, opt => opt.MapFrom(src => src.TopicType))
                .ForMember(dest => dest.Payload, opt => opt.MapFrom(src => src.Payload));
            // No properties to ignore since all are mapped

            // ---------------------------------------------
            // 7. NotificationRequestDto -> Notification
            // ---------------------------------------------
            CreateMap<NotificationRequestDto, Notification>()
                .ForMember(dest => dest.NotificationId, opt => opt.MapFrom(src => Guid.NewGuid().ToString()))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp))

                // Explicitly ignore unmapped properties
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());

            // ---------------------------------------------
            // 8. LocationRequestDto -> Location
            // ---------------------------------------------
            CreateMap<LocationRequestDto, Location>()
                .ForMember(dest => dest.TrackerId, opt => opt.MapFrom(src => src.TrackerId))
                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Latitude))
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Longitude))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp))

                // Explicitly ignore unmapped properties
                .ForMember(dest => dest.LocationId, opt => opt.Ignore())
                .ForMember(dest => dest.Tracker, opt => opt.Ignore());

            // ---------------------------------------------
            // 9. MotionRequestDto -> Motion
            // ---------------------------------------------
            CreateMap<MotionRequestDto, Motion>()
                .ForMember(dest => dest.TrackerId, opt => opt.MapFrom(src => src.TrackerId))
                .ForMember(dest => dest.MotionDetected, opt => opt.MapFrom(src => src.MotionDetected))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp))

                // Explicitly ignore unmapped properties
                .ForMember(dest => dest.MotionId, opt => opt.Ignore())
                .ForMember(dest => dest.Tracker, opt => opt.Ignore());
        }
    }
}
