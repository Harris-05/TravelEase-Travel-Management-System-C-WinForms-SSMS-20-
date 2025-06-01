CREATE DATABASE TravelEase;
USE TravelEase;

-- USERS TABLE
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    FullName VARCHAR(100),
    Email VARCHAR(100) UNIQUE,
    PasswordHash VARCHAR(255),
    Role VARCHAR(20),
    DateOfBirth DATETIME,
    Nationality VARCHAR(50),
    Gender VARCHAR(10),
    CreatedAt DATETIME DEFAULT GETDATE(),
    IsApproved BIT DEFAULT 0
);

select * from Users

-- DESTINATIONS
CREATE TABLE Destinations (
    DestinationID INT PRIMARY KEY IDENTITY(1,1),
    City VARCHAR(50),
    Country VARCHAR(50),
    Description TEXT
);

-- TOUR CATEGORIES
CREATE TABLE TourCategories (
    CategoryID INT PRIMARY KEY IDENTITY(1,1),
    CategoryName VARCHAR(50)
);

-- TRAVELERS
CREATE TABLE Travelers (
    TravelerID INT PRIMARY KEY,
    Preferences TEXT,
    FOREIGN KEY (TravelerID) REFERENCES Users(UserID)
);

INSERT INTO Travelers (TravelerID, Preferences)
SELECT UserID, ''
FROM Users
WHERE Role = 'Traveler'
  AND UserID NOT IN (SELECT TravelerID FROM Travelers);

select * from Travelers;
-- TOUR OPERATORS
CREATE TABLE TourOperators (
    OperatorID INT PRIMARY KEY,
    CompanyName VARCHAR(100),
    ContactInfo VARCHAR(255),
    FOREIGN KEY (OperatorID) REFERENCES Users(UserID)
);

-- SERVICE PROVIDERS
CREATE TABLE ServiceProviders (
    ProviderID INT PRIMARY KEY,
    ProviderType VARCHAR(50),
    OrganizationName VARCHAR(100),
    ContactInfo VARCHAR(255),
    FOREIGN KEY (ProviderID) REFERENCES Users(UserID)
);

-- TRIPS
CREATE TABLE Trips (
    TripID INT PRIMARY KEY IDENTITY(1,1),
    OperatorID INT,
    CategoryID INT,
    DestinationID INT,
    Title VARCHAR(100),
    Description TEXT,
    StartDate DATETIME,
    EndDate DATETIME,
    PricePerPerson DECIMAL(10,2),
    Capacity INT,
    SustainabilityScore INT,
    AccessibilityFeatures TEXT,
    FOREIGN KEY (OperatorID) REFERENCES TourOperators(OperatorID),
    FOREIGN KEY (CategoryID) REFERENCES TourCategories(CategoryID),
    FOREIGN KEY (DestinationID) REFERENCES Destinations(DestinationID)
);

-- OPERATOR CATEGORIES (many-to-many)
CREATE TABLE OperatorCategories (
    OperatorID INT,
    CategoryID INT,
    PRIMARY KEY (OperatorID, CategoryID),
    FOREIGN KEY (OperatorID) REFERENCES TourOperators(OperatorID),
    FOREIGN KEY (CategoryID) REFERENCES TourCategories(CategoryID)
);

-- TRIP DESTINATIONS (many-to-many)
CREATE TABLE TripDestinations (
    TripID INT,
    DestinationName VARCHAR(100),
    PRIMARY KEY (TripID, DestinationName),
    FOREIGN KEY (TripID) REFERENCES Trips(TripID)
);

-- TRIP INCLUSIONS
CREATE TABLE TripInclusions (
    InclusionID INT PRIMARY KEY IDENTITY(1,1),
    TripID INT,
    InclusionType VARCHAR(50),
    Description TEXT,
    FOREIGN KEY (TripID) REFERENCES Trips(TripID)
);

-- BOOKINGS
CREATE TABLE Bookings (
    BookingID INT PRIMARY KEY IDENTITY(1,1),
    TravelerID INT,
    TripID INT,
    BookingDate DATETIME DEFAULT GETDATE(),
    NumberOfPeople INT,
    TotalAmount DECIMAL(10,2),
    Status VARCHAR(20),
    FOREIGN KEY (TravelerID) REFERENCES Travelers(TravelerID),
    FOREIGN KEY (TripID) REFERENCES Trips(TripID)
);

-- ABANDONED BOOKINGS
CREATE TABLE AbandonedBookings (
    AbandonedID INT PRIMARY KEY IDENTITY(1,1),
    TravelerID INT,
    TripID INT,
    Reason VARCHAR(255),
    CreatedAt DATETIME,
    FOREIGN KEY (TravelerID) REFERENCES Travelers(TravelerID),
    FOREIGN KEY (TripID) REFERENCES Trips(TripID)
);

-- PAYMENTS
CREATE TABLE Payments (
    PaymentID INT PRIMARY KEY IDENTITY(1,1),
    BookingID INT,
    Amount DECIMAL(10,2),
    PaymentMethod VARCHAR(50),
    Status VARCHAR(20),
    PaymentDate DATETIME,
    FOREIGN KEY (BookingID) REFERENCES Bookings(BookingID)
);

-- CANCELLATIONS
CREATE TABLE Cancellations (
    CancellationID INT PRIMARY KEY IDENTITY(1,1),
    BookingID INT,
    Reason TEXT,
    CancelledAt DATETIME,
    RefundAmount DECIMAL(10,2),
    FOREIGN KEY (BookingID) REFERENCES Bookings(BookingID)
);

-- REFUND REQUESTS
CREATE TABLE RefundRequests (
    RefundRequestID INT PRIMARY KEY IDENTITY(1,1),
    CancellationID INT,
    RequestedAmount DECIMAL(10,2),
    RequestDate DATETIME DEFAULT GETDATE(),
    Status VARCHAR(50),
    FOREIGN KEY (CancellationID) REFERENCES Cancellations(CancellationID)
);

-- DIGITAL PASSES
CREATE TABLE DigitalPasses (
    PassID INT PRIMARY KEY IDENTITY(1,1),
    BookingID INT,
    PassType VARCHAR(50),
    FileURL VARCHAR(255),
    FOREIGN KEY (BookingID) REFERENCES Bookings(BookingID)
);

-- SERVICES PROVIDED
CREATE TABLE ServicesProvided (
    ServiceID INT PRIMARY KEY IDENTITY(1,1),
    ProviderID INT,
    ServiceType VARCHAR(50),
    Title VARCHAR(100),
    Description TEXT,
    Price DECIMAL(10,2),
    Availability INT,
    FOREIGN KEY (ProviderID) REFERENCES ServiceProviders(ProviderID)
);
Alter
-- TRIP SERVICES
CREATE TABLE TripServices (
    TripServiceID INT PRIMARY KEY IDENTITY(1,1),
    TripID INT,
    ServiceID INT,
    Status VARCHAR(20),
    FOREIGN KEY (TripID) REFERENCES Trips(TripID),
    FOREIGN KEY (ServiceID) REFERENCES ServicesProvided(ServiceID)
);
SELECT * FROM Travelers WHERE TravelerID NOT IN (SELECT UserID FROM Users WHERE Role = 'traveler');

-- CLIENT SERVICES
CREATE TABLE ClientServices (
    ClientServiceID INT PRIMARY KEY IDENTITY(1,1),
    TripID INT,
    ServiceID INT,
    Status VARCHAR(50),
    FOREIGN KEY (TripID) REFERENCES Trips(TripID),
    FOREIGN KEY (ServiceID) REFERENCES ServicesProvided(ServiceID)
);

-- REVIEWS
CREATE TABLE Reviews (
    ReviewID INT PRIMARY KEY IDENTITY(1,1),
    TravelerID INT,
    TripID INT,
    ProviderID INT NULL,
    Rating INT,
    Comment TEXT,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (TravelerID) REFERENCES Travelers(TravelerID),
    FOREIGN KEY (TripID) REFERENCES Trips(TripID),
    FOREIGN KEY (ProviderID) REFERENCES ServiceProviders(ProviderID)
);

-- PREFERENCES
CREATE TABLE Preferences (
    PreferenceID INT PRIMARY KEY IDENTITY(1,1),
    TravelerID INT,
    PreferenceType VARCHAR(100),
    Value VARCHAR(255),
    FOREIGN KEY (TravelerID) REFERENCES Travelers(TravelerID)
);

-- AUDIT TRAIL
CREATE TABLE AuditTrail (
    AuditID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT,
    Action VARCHAR(255),
    Timestamp DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

SELECT * FROM Preferences;

--Query For Booking History
SELECT T.TripName, B.BookingDate, B.Status
FROM Bookings B
JOIN Trips T ON B.TripID = T.TripID
WHERE B.TravelerID = @TravelerID
ORDER BY B.BookingDate DESC;

--
INSERT INTO Destinations (City, Country, Description)
VALUES ('Hunza', 'Pakistan', 'Breathtaking valley with mountains and lakes');
select * from Destinations


INSERT INTO TourCategories (CategoryName)
VALUES ('Adventure');
select * from TourCategories

SELECT name 
FROM sys.foreign_keys 
WHERE parent_object_id = OBJECT_ID('Trips');

ALTER TABLE Trips DROP CONSTRAINT FK__Trips__Destinati__108B795B;

ALTER TABLE Trips DROP COLUMN DestinationID;
ALTER TABLE Trips ADD DestinationName VARCHAR(100);

SELECT name 
FROM sys.foreign_keys 
WHERE parent_object_id = OBJECT_ID('TripDestinations');

--2 removing fk from trip destination
-- Drop foreign key constraint
-- Step 1: Drop the primary key constraint
ALTER TABLE TripDestinations
DROP CONSTRAINT PK_TripDest_8C698F54FB8B605E; -- Use your actual constraint name if different

-- Step 2: Drop the foreign key constraint (if not already dropped)
ALTER TABLE TripDestinations
DROP CONSTRAINT FK_TripDestinations_DestinationID;

-- Step 3: Drop the DestinationID column
ALTER TABLE TripDestinations
DROP COLUMN DestinationID;

-- Step 4: Add the DestinationName column
ALTER TABLE TripDestinations
ADD DestinationName VARCHAR(100);

-- Step 5: Add new primary key on TripID only (or combine with DestinationName if needed)
ALTER TABLE TripDestinations
ADD CONSTRAINT PK_TripDestinations PRIMARY KEY (TripID);


DROP TABLE Destinations;

SELECT name, type_desc 
FROM sys.key_constraints 
WHERE parent_object_id = OBJECT_ID('TripDestinations');

-- Get foreign key constraint names
SELECT name 
FROM sys.foreign_keys 
WHERE parent_object_id = OBJECT_ID('TripDestinations');


SELECT name 
FROM sys.foreign_keys 
WHERE parent_object_id = OBJECT_ID('Trips');

--1 removing fk destination from trip
-- Drop foreign key constraint (you may need to know the name, but assuming it's default-named)
ALTER TABLE Trips DROP CONSTRAINT FK__Trips__CategoryI__0F975522;

-- Drop DestinationID column
ALTER TABLE Trips DROP COLUMN CategoryID;

-- Add new DestinationName column
ALTER TABLE Trips ADD Category VARCHAR(100);

INSERT INTO Users (FullName, Email, PasswordHash, Role, DateOfBirth, Nationality, Gender)
VALUES ('Alice Traveler', 'alice@travel.com', 'hash', 'Traveler', '1990-01-01', 'USA', 'F');

INSERT INTO Users (FullName, Email, PasswordHash, Role, DateOfBirth, Nationality, Gender)
VALUES ('Bob Traveler', 'bob@travel.com', 'hash', 'Traveler', '1991-02-02', 'UK', 'M');

INSERT INTO Users (FullName, Email, PasswordHash, Role, DateOfBirth, Nationality, Gender)
VALUES ('Carol Traveler', 'carol@travel.com', 'hash', 'Traveler', '1992-03-03', 'Canada', 'F');

INSERT INTO Users (FullName, Email, PasswordHash, Role, DateOfBirth, Nationality, Gender)
VALUES ('Dave Traveler', 'dave@travel.com', 'hash', 'Traveler', '1993-04-04', 'Australia', 'M');

INSERT INTO Users (FullName, Email, PasswordHash, Role, DateOfBirth, Nationality, Gender)
VALUES ('Eve Operator', 'eve@op.com', 'hash', 'Operator', '1980-05-05', 'USA', 'F');

INSERT INTO Users (FullName, Email, PasswordHash, Role, DateOfBirth, Nationality, Gender)
VALUES ('Frank Operator', 'frank@op.com', 'hash', 'Operator', '1981-06-06', 'UK', 'M');

INSERT INTO Users (FullName, Email, PasswordHash, Role, DateOfBirth, Nationality, Gender)
VALUES ('Grace Operator', 'grace@op.com', 'hash', 'Operator', '1982-07-07', 'Canada', 'F');

INSERT INTO Users (FullName, Email, PasswordHash, Role, DateOfBirth, Nationality, Gender)
VALUES ('Heidi Provider', 'heidi@prov.com', 'hash', 'Provider', '1983-08-08', 'USA', 'F');

INSERT INTO Users (FullName, Email, PasswordHash, Role, DateOfBirth, Nationality, Gender)
VALUES ('Ivan Provider', 'ivan@prov.com', 'hash', 'Provider', '1984-09-09', 'UK', 'M');

INSERT INTO Users (FullName, Email, PasswordHash, Role, DateOfBirth, Nationality, Gender)
VALUES ('Judy Provider', 'judy@prov.com', 'hash', 'Provider', '1985-10-10', 'Canada', 'F');


-- TRAVELERS (UserID 1-4)
INSERT INTO Travelers (TravelerID, Preferences) VALUES (1, 'Beach');
INSERT INTO Travelers (TravelerID, Preferences) VALUES (2, 'Mountains');
INSERT INTO Travelers (TravelerID, Preferences) VALUES (3, 'City');
INSERT INTO Travelers (TravelerID, Preferences) VALUES (4, 'Adventure');


-- OPERATORS (UserID 5-7)
INSERT INTO TourOperators (OperatorID, CompanyName, ContactInfo) VALUES (5, 'Eve Tours', 'eve@op.com');
INSERT INTO TourOperators (OperatorID, CompanyName, ContactInfo) VALUES (6, 'Frank Expeditions', 'frank@op.com');
INSERT INTO TourOperators (OperatorID, CompanyName, ContactInfo) VALUES (7, 'Grace Adventures', 'grace@op.com');

-- PROVIDERS (UserID 8-10)
INSERT INTO ServiceProviders (ProviderID, ProviderType, OrganizationName, ContactInfo) VALUES (8, 'Hotel', 'Heidi Hotels', 'heidi@prov.com');
INSERT INTO ServiceProviders (ProviderID, ProviderType, OrganizationName, ContactInfo) VALUES (9, 'Transport', 'Ivan Transport', 'ivan@prov.com');
INSERT INTO ServiceProviders (ProviderID, ProviderType, OrganizationName, ContactInfo) VALUES (10, 'Guide', 'Judy Guides', 'judy@prov.com');


-- TOUR CATEGORIES (3)
INSERT INTO TourCategories (CategoryName) VALUES ('Cultural');
INSERT INTO TourCategories (CategoryName) VALUES ('Adventure');
INSERT INTO TourCategories (CategoryName) VALUES ('Relaxation');


-- TRIPS (5) -- Now uses Category and DestinationName as strings
INSERT INTO Trips (OperatorID, Category, DestinationName, Title, Description, StartDate, EndDate, PricePerPerson, Capacity, SustainabilityScore, AccessibilityFeatures)
VALUES (5, 'Cultural', 'Paris', 'Paris Explorer', 'See the best of Paris', '2024-07-01', '2024-07-07', 1200, 20, 8, 'Wheelchair');

INSERT INTO Trips (OperatorID, Category, DestinationName, Title, Description, StartDate, EndDate, PricePerPerson, Capacity, SustainabilityScore, AccessibilityFeatures)
VALUES (6, 'Adventure', 'Cairo', 'Egypt Adventure', 'Pyramids and more', '2024-08-10', '2024-08-20', 1500, 15, 7, 'None');

INSERT INTO Trips (OperatorID, Category, DestinationName, Title, Description, StartDate, EndDate, PricePerPerson, Capacity, SustainabilityScore, AccessibilityFeatures)
VALUES (7, 'Relaxation', 'Sydney', 'Sydney Relax', 'Relax in Sydney', '2024-09-05', '2024-09-12', 1100, 10, 9, 'Wheelchair');

INSERT INTO Trips (OperatorID, Category, DestinationName, Title, Description, StartDate, EndDate, PricePerPerson, Capacity, SustainabilityScore, AccessibilityFeatures)
VALUES (5, 'Cultural', 'Toronto', 'Toronto Culture', 'Museums and more', '2024-10-01', '2024-10-08', 1000, 12, 8, 'None');

INSERT INTO Trips (OperatorID, Category, DestinationName, Title, Description, StartDate, EndDate, PricePerPerson, Capacity, SustainabilityScore, AccessibilityFeatures)
VALUES (6, 'Adventure', 'Paris', 'Paris Adventure', 'Adventure in Paris', '2024-11-01', '2024-11-10', 1300, 18, 7, 'None');

INSERT INTO OperatorCategories (OperatorID, CategoryID) VALUES (5, 1);
INSERT INTO OperatorCategories (OperatorID, CategoryID) VALUES (5, 2);
INSERT INTO OperatorCategories (OperatorID, CategoryID) VALUES (6, 2);
INSERT INTO OperatorCategories (OperatorID, CategoryID) VALUES (7, 3);


-- TRIP DESTINATIONS (link trips to destinations, now uses DestinationName)
INSERT INTO TripDestinations (TripID, DestinationName) VALUES (1, 'Paris');
INSERT INTO TripDestinations (TripID, DestinationName) VALUES (2, 'Cairo');
INSERT INTO TripDestinations (TripID, DestinationName) VALUES (3, 'Sydney');
INSERT INTO TripDestinations (TripID, DestinationName) VALUES (4, 'Toronto');
INSERT INTO TripDestinations (TripID, DestinationName) VALUES (5, 'Paris');


-- TRIP INCLUSIONS (2 per trip)
INSERT INTO TripInclusions (TripID, InclusionType, Description) VALUES (1, 'Meal', 'Breakfast included');
INSERT INTO TripInclusions (TripID, InclusionType, Description) VALUES (1, 'Guide', 'English guide');
INSERT INTO TripInclusions (TripID, InclusionType, Description) VALUES (2, 'Meal', 'Lunch included');
INSERT INTO TripInclusions (TripID, InclusionType, Description) VALUES (2, 'Transport', 'Airport pickup');
INSERT INTO TripInclusions (TripID, InclusionType, Description) VALUES (3, 'Spa', 'Free spa access');
INSERT INTO TripInclusions (TripID, InclusionType, Description) VALUES (3, 'Meal', 'Dinner included');
INSERT INTO TripInclusions (TripID, InclusionType, Description) VALUES (4, 'Museum', 'Museum tickets');
INSERT INTO TripInclusions (TripID, InclusionType, Description) VALUES (4, 'Guide', 'French guide');
INSERT INTO TripInclusions (TripID, InclusionType, Description) VALUES (5, 'Adventure', 'Climbing');
INSERT INTO TripInclusions (TripID, InclusionType, Description) VALUES (5, 'Meal', 'Snacks included');


-- SERVICES PROVIDED (6)
INSERT INTO ServicesProvided (ProviderID, ServiceType, Title, Description, Price, Availability) VALUES (8, 'Hotel', 'Hotel Paris', 'Central Paris hotel', 200, 10);
INSERT INTO ServicesProvided (ProviderID, ServiceType, Title, Description, Price, Availability) VALUES (8, 'Hotel', 'Hotel Sydney', 'Sydney downtown', 180, 8);
INSERT INTO ServicesProvided (ProviderID, ServiceType, Title, Description, Price, Availability) VALUES (9, 'Transport', 'Cairo Shuttle', 'Airport to hotel', 50, 20);
INSERT INTO ServicesProvided (ProviderID, ServiceType, Title, Description, Price, Availability) VALUES (9, 'Transport', 'Toronto Shuttle', 'City tour', 60, 15);
INSERT INTO ServicesProvided (ProviderID, ServiceType, Title, Description, Price, Availability) VALUES (10, 'Guide', 'Paris Guide', 'Expert guide', 100, 5);
INSERT INTO ServicesProvided (ProviderID, ServiceType, Title, Description, Price, Availability) VALUES (10, 'Guide', 'Egypt Guide', 'Pyramids expert', 120, 6);

-- TRIP SERVICES (link trips to services)
INSERT INTO TripServices (TripID, ServiceID, Status) VALUES (1, 1, 'active');
INSERT INTO TripServices (TripID, ServiceID, Status) VALUES (2, 3, 'active');
INSERT INTO TripServices (TripID, ServiceID, Status) VALUES (3, 2, 'active');
INSERT INTO TripServices (TripID, ServiceID, Status) VALUES (4, 4, 'active');
INSERT INTO TripServices (TripID, ServiceID, Status) VALUES (5, 5, 'active');


-- CLIENT SERVICES (link trips to services for clients)
INSERT INTO ClientServices (TripID, ServiceID, Status) VALUES (1, 1, 'pending');
INSERT INTO ClientServices (TripID, ServiceID, Status) VALUES (2, 3, 'approved');
INSERT INTO ClientServices (TripID, ServiceID, Status) VALUES (3, 2, 'pending');
INSERT INTO ClientServices (TripID, ServiceID, Status) VALUES (4, 4, 'approved');
INSERT INTO ClientServices (TripID, ServiceID, Status) VALUES (5, 5, 'pending');


-- BOOKINGS (8)
INSERT INTO Bookings (TravelerID, TripID, BookingDate, NumberOfPeople, TotalAmount, Status) VALUES (61, 1, GETDATE(), 2, 2400, 'pending');
INSERT INTO Bookings (TravelerID, TripID, BookingDate, NumberOfPeople, TotalAmount, Status) VALUES (2, 2, GETDATE(), 1, 1500, 'confirmed');
INSERT INTO Bookings (TravelerID, TripID, BookingDate, NumberOfPeople, TotalAmount, Status) VALUES (3, 3, GETDATE(), 3, 3300, 'pending');
INSERT INTO Bookings (TravelerID, TripID, BookingDate, NumberOfPeople, TotalAmount, Status) VALUES (4, 4, GETDATE(), 2, 2000, 'cancelled');
INSERT INTO Bookings (TravelerID, TripID, BookingDate, NumberOfPeople, TotalAmount, Status) VALUES (61, 5, GETDATE(), 1, 1300, 'pending');
INSERT INTO Bookings (TravelerID, TripID, BookingDate, NumberOfPeople, TotalAmount, Status) VALUES (2, 1, GETDATE(), 2, 2400, 'confirmed');
INSERT INTO Bookings (TravelerID, TripID, BookingDate, NumberOfPeople, TotalAmount, Status) VALUES (3, 2, GETDATE(), 1, 1500, 'pending');
INSERT INTO Bookings (TravelerID, TripID, BookingDate, NumberOfPeople, TotalAmount, Status) VALUES (4, 3, GETDATE(), 2, 2200, 'pending');


-- PAYMENTS (6)
INSERT INTO Payments (BookingID, Amount, PaymentMethod, Status, PaymentDate) VALUES (1, 2400, 'Credit Card', 'paid', GETDATE());
INSERT INTO Payments (BookingID, Amount, PaymentMethod, Status, PaymentDate) VALUES (2, 1500, 'Credit Card', 'paid', GETDATE());
INSERT INTO Payments (BookingID, Amount, PaymentMethod, Status, PaymentDate) VALUES (3, 3300, 'Cash', 'pending', GETDATE());
INSERT INTO Payments (BookingID, Amount, PaymentMethod, Status, PaymentDate) VALUES (4, 2000, 'Credit Card', 'refunded', GETDATE());
INSERT INTO Payments (BookingID, Amount, PaymentMethod, Status, PaymentDate) VALUES (5, 1300, 'Cash', 'pending', GETDATE());
INSERT INTO Payments (BookingID, Amount, PaymentMethod, Status, PaymentDate) VALUES (6, 2400, 'Credit Card', 'paid', GETDATE());


-- CANCELLATIONS (1)
INSERT INTO Cancellations (BookingID, Reason, CancelledAt, RefundAmount) VALUES (4, 'Client cancelled', GETDATE(), 2000);


-- REFUND REQUESTS (1)
INSERT INTO RefundRequests (CancellationID, RequestedAmount, Status) VALUES (1, 2000, 'pending');


-- DIGITAL PASSES (2)
INSERT INTO DigitalPasses (BookingID, PassType, FileURL) VALUES (1, 'QR', 'url1');
INSERT INTO DigitalPasses (BookingID, PassType, FileURL) VALUES (2, 'PDF', 'url2');


-- REVIEWS (2)
INSERT INTO Reviews (TravelerID, TripID, ProviderID, Rating, Comment) VALUES (1, 1, 8, 5, 'Great trip!');
INSERT INTO Reviews (TravelerID, TripID, ProviderID, Rating, Comment) VALUES (2, 2, 9, 4, 'Nice experience.');


-- PREFERENCES (2)
INSERT INTO Preferences (TravelerID, PreferenceType, Value) VALUES (1, 'Food', 'Vegetarian');
INSERT INTO Preferences (TravelerID, PreferenceType, Value) VALUES (2, 'Room', 'Suite');

-- AUDIT TRAIL (2)
INSERT INTO AuditTrail (UserID, Action) VALUES (1, 'Login');
INSERT INTO AuditTrail (UserID, Action) VALUES (2, 'Booking created');

SELECT 
    T.Title AS [Trip],
    
    T.DestinationName AS [Destination],
    T.Category AS [Category],
    B.Status AS [Status],
    B.BookingID AS [Booking Ref.],
    'N/A' AS [Policy]
FROM Bookings B
INNER JOIN Trips T ON B.TripID = T.TripID
WHERE B.TravelerID = 1
ORDER BY T.StartDate ASC;
select * from users
