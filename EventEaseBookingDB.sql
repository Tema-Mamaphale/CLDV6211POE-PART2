-- Drop existing tables and view if they exist to avoid conflicts
IF OBJECT_ID('ConsolidatedBookingView', 'V') IS NOT NULL
    DROP VIEW ConsolidatedBookingView;

IF OBJECT_ID('Booking', 'U') IS NOT NULL
    DROP TABLE Booking;

IF OBJECT_ID('Event', 'U') IS NOT NULL
    DROP TABLE Event;

IF OBJECT_ID('Venue', 'U') IS NOT NULL
    DROP TABLE Venue;

-- Create Venue table
CREATE TABLE Venue (
    VenueId INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    VenueName VARCHAR(100) NOT NULL,
    Location VARCHAR(100) NOT NULL,
    Capacity INT NOT NULL,
    ImageUrl VARCHAR(MAX)
);

-- Create Event table without the Description field
CREATE TABLE Event (
    EventId INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    EventName VARCHAR(100) NOT NULL,
    EventDate DATE NOT NULL,
    VenueId INT NOT NULL,
    FOREIGN KEY (VenueId) REFERENCES Venue(VenueId) ON DELETE CASCADE
);

-- Create Booking table with CASCADE DELETE for Event only
CREATE TABLE Booking (
    BookingId INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    EventId INT NOT NULL,
    VenueId INT NOT NULL,
    BookingDate DATE NOT NULL,
    FOREIGN KEY (EventId) REFERENCES Event(EventId) ON DELETE CASCADE,
    FOREIGN KEY (VenueId) REFERENCES Venue(VenueId) -- No cascading delete here
);

-- Create Consolidated Booking View without the Description field
CREATE VIEW ConsolidatedBookingView AS
SELECT 
    b.BookingId,
    b.BookingDate,
    e.EventName,
    e.EventDate,
    v.VenueName,
    v.Location,
    v.Capacity,
    v.ImageUrl
FROM 
    Booking b
JOIN 
    Event e ON b.EventId = e.EventId
JOIN 
    Venue v ON b.VenueId = v.VenueId;

-- Insert venues
INSERT INTO Venue (VenueName, Location, Capacity, ImageUrl) VALUES
('Big Orchid', 'Knysna', 500, 'bigorchid.jpg'),
('Lotus Lake', 'Umhlanga', 600, 'lotuslake.jpg'),
('The Boardwalk Venue', 'Cape Town', 800, 'theboardwalkvenue.jpg');

-- Insert events without the Description field
INSERT INTO Event (EventName, EventDate, VenueId) VALUES
('Vowel Renewal', '2025-09-24', 1),
('Wedding', '2025-10-02', 1),
('Shareholders Conference', '2025-04-06', 3);

-- Insert bookings
INSERT INTO Booking (EventId, VenueId, BookingDate) VALUES
(2, 1, '2025-08-28'),
(1, 2, '2025-11-15'),
(2, 1, '2025-05-20');

-- View the consolidated results
SELECT * FROM ConsolidatedBookingView;
