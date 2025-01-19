-- ============================================
-- Step 1: Switch to ArgusDB Database
-- (If you already have ArgusDB created on Azure, 
--  omit the CREATE DATABASE statement and just do USE ArgusDB)
-- ============================================

/*
CREATE DATABASE ArgusDB;
GO
*/

USE ArgusDB;
GO

-- ============================================
-- Step 2: Drop Existing Tables in Correct Order
-- ============================================

-- Drop Notifications Table (References Users)
IF OBJECT_ID('dbo.Notifications', 'U') IS NOT NULL
    DROP TABLE dbo.Notifications;
GO

-- Drop MQTT Table (References Trackers)
IF OBJECT_ID('dbo.MQTT', 'U') IS NOT NULL
    DROP TABLE dbo.MQTT;
GO

-- Drop Locks Table (References Users and Trackers)
IF OBJECT_ID('dbo.Locks', 'U') IS NOT NULL
    DROP TABLE dbo.Locks;
GO

-- Drop Motions Table (References Trackers)
IF OBJECT_ID('dbo.Motions', 'U') IS NOT NULL
    DROP TABLE dbo.Motions;
GO

-- Drop Locations Table (References Trackers)
IF OBJECT_ID('dbo.Locations', 'U') IS NOT NULL
    DROP TABLE dbo.Locations;
GO

-- Drop Trackers Table (References Users)
IF OBJECT_ID('dbo.Trackers', 'U') IS NOT NULL
    DROP TABLE dbo.Trackers;
GO

-- Drop Users Table
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL
    DROP TABLE dbo.Users;
GO


-- ============================================
-- Step 3: Create Tables as per Updated Models
-- ============================================
-- 1) Create Users Table
CREATE TABLE Users (
    FirebaseUID              NVARCHAR(128)  PRIMARY KEY,
    Email                    NVARCHAR(100)  NOT NULL,
    NotificationPreferences  NVARCHAR(MAX),  -- JSON or free-form text
    Role                     NVARCHAR(5)    NOT NULL
        CHECK (Role IN ('admin','user'))
);
GO

-- 2) Create Trackers Table
CREATE TABLE Trackers (
    TrackerId         NVARCHAR(128) PRIMARY KEY,
    FirebaseUID       NVARCHAR(128) NULL,  -- Changed from NOT NULL to NULL
    Email             NVARCHAR(100),      -- optional
    MqttUsername      NVARCHAR(100),
    MqttPassword      NVARCHAR(100),
    BrokerUrl         NVARCHAR(255),
    Port              INT CHECK (Port BETWEEN 1 AND 65535),
    LockState         NVARCHAR(8) NOT NULL
        CHECK (LockState IN ('locked','unlocked')),
    DesiredLockState  NVARCHAR(8) NOT NULL
        CHECK (DesiredLockState IN ('locked','unlocked')),
    LastKnownLocation NVARCHAR(MAX),  -- e.g., JSON with lat, lon, timestamp
    LastUpdated       DATETIME NOT NULL,
    CreatedAt         DATETIME NOT NULL DEFAULT GETUTCDATE(), -- Added CreatedAt
    
    FOREIGN KEY (FirebaseUID) REFERENCES Users(FirebaseUID)
);
GO

-- 3) Create Locks Table
CREATE TABLE Locks (
    LockId       NVARCHAR(100) NOT NULL PRIMARY KEY,
    TrackerId    NVARCHAR(128) NOT NULL,
    FirebaseUID  NVARCHAR(128) NOT NULL,  -- Changed from NVARCHAR(100) to NVARCHAR(128)
    Email        NVARCHAR(255) NULL,
    Status       NVARCHAR(8)   NOT NULL
        CHECK (Status IN ('locked','unlocked')),
    LastUpdated  DATETIME      NOT NULL,
    
    FOREIGN KEY (TrackerId) REFERENCES Trackers(TrackerId),
    FOREIGN KEY (FirebaseUID) REFERENCES Users(FirebaseUID)
);
GO

-- 4) Create Motions Table
CREATE TABLE Motions (
    MotionId        INT IDENTITY(1,1) PRIMARY KEY,
    TrackerId       NVARCHAR(128) NOT NULL, 
    MotionDetected  BIT NOT NULL,
    Timestamp       DATETIME NOT NULL,
    
    FOREIGN KEY (TrackerId) REFERENCES Trackers(TrackerId)
);
GO

-- 5) Create Locations Table
CREATE TABLE Locations (
    LocationId  INT IDENTITY(1,1) PRIMARY KEY,
    TrackerId   NVARCHAR(128) NOT NULL,
    Latitude    FLOAT NOT NULL,
    Longitude   FLOAT NOT NULL,
    Timestamp   DATETIME NOT NULL,
    
    FOREIGN KEY (TrackerId) REFERENCES Trackers(TrackerId)
);
GO

-- 6) Create MQTT Table
CREATE TABLE MQTT (
    MqttId               NVARCHAR(128) PRIMARY KEY,
    TrackerId            NVARCHAR(128) NOT NULL,
    MqttTopic            NVARCHAR(256) NOT NULL,
    Qos                  INT NOT NULL CHECK (Qos BETWEEN 0 AND 2),
    LastMessageTimestamp DATETIME NOT NULL,
    Retain               BIT NOT NULL,
    Status               NVARCHAR(50) NOT NULL,
    
    FOREIGN KEY (TrackerId) REFERENCES Trackers(TrackerId)
);
GO

-- 7) Create Notifications Table
CREATE TABLE Notifications (
    NotificationId NVARCHAR(128) PRIMARY KEY,
    Type           NVARCHAR(50),
    Timestamp      DATETIME NOT NULL,
    Message        NVARCHAR(MAX),
    UserId         NVARCHAR(128),      -- references Users.FirebaseUID
    
    FOREIGN KEY (UserId) REFERENCES Users(FirebaseUID)
);
GO


-- ============================================
-- Step 4: Insert Test Data into Tables
-- ============================================

-- 4.1 Insert sample Users
INSERT INTO Users (FirebaseUID, Email, NotificationPreferences, Role)
VALUES
('UID_Alpha', 'alpha@example.com', '{ "EmailNotifications": true, "SmsNotifications": false }', 'admin'),
('UID_Beta',  'beta@example.com',  NULL, 'user');
GO

-- 4.2 Insert sample Trackers
INSERT INTO Trackers (
    TrackerId, 
    FirebaseUID, 
    Email, 
    MqttUsername, 
    MqttPassword,
    BrokerUrl, 
    Port, 
    LockState, 
    DesiredLockState,
    LastKnownLocation, 
    LastUpdated
)
VALUES
('Tracker_001', 'UID_Alpha', 'alpha@example.com', 'alphaMQTTUser', 'alphaMQTTPass',
 'mqtt://broker.example.com', 1883, 'unlocked', 'unlocked',
 NULL, GETDATE()),

('Tracker_002', 'UID_Beta', 'beta@example.com', 'betaMQTTUser', 'betaMQTTPass',
 'mqtt://broker.example.com', 1883, 'locked', 'locked',
 '{"lat":45.1,"lon":-118.4,"ts":"2025-01-17T03:00:00Z"}', GETDATE());
GO

-- 4.3 Insert sample Locks
-- Updated to match the Lock model constraints (LockId <= 100 chars, etc.)
INSERT INTO Locks (LockId, TrackerId, FirebaseUID, Email, Status, LastUpdated)
VALUES
('Lock_XYZ', 'Tracker_001', 'UID_Alpha', 'alpha@example.com', 'locked', GETDATE()),
('Lock_ABC', 'Tracker_002', 'UID_Beta',  'beta@example.com',  'unlocked', GETDATE());
GO

-- 4.4 Insert sample Notifications
INSERT INTO Notifications (NotificationId, Type, Timestamp, Message, UserId)
VALUES
('Notif_001', 'motion',   GETDATE(), 'Motion detected on Tracker_001.', 'UID_Alpha'),
('Notif_002', 'lockState',GETDATE(), 'Tracker_002 locked.',             'UID_Beta');
GO

-- 4.5 Insert sample Locations
INSERT INTO Locations (TrackerId, Latitude, Longitude, Timestamp)
VALUES
('Tracker_001', 37.7749, -122.4194, GETDATE()),  -- e.g., San Francisco
('Tracker_002', 34.0522, -118.2437, GETDATE());  -- e.g., Los Angeles
GO

-- 4.6 Insert sample Motions
INSERT INTO Motions (TrackerId, MotionDetected, Timestamp)
VALUES
('Tracker_001', 1, GETDATE()),
('Tracker_002', 0, GETDATE());
GO

-- 4.7 Insert sample MQTT configurations
INSERT INTO MQTT (MqttId, TrackerId, MqttTopic, Qos, LastMessageTimestamp, Retain, Status)
VALUES
('MQTT_001', 'Tracker_001', 'tracker/001/telemetry', 0, GETDATE(), 0, 'connected'),
('MQTT_002', 'Tracker_002', 'tracker/002/telemetry', 1, GETDATE(), 1, 'disconnected');
GO
