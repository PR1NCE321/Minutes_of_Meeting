USE [MOM_DB]
GO

-- Disable Ref. Constraints to allow clean delete
ALTER TABLE [dbo].[MOM_MeetingMember] NOCHECK CONSTRAINT ALL
ALTER TABLE [dbo].[MOM_Meetings] NOCHECK CONSTRAINT ALL
ALTER TABLE [dbo].[MOM_Staff] NOCHECK CONSTRAINT ALL
GO

-- 1. CLEANUP (Optional: Remove if you want to keep existing data)
DELETE FROM [dbo].[MOM_MeetingMember]
DELETE FROM [dbo].[MOM_Meetings]
DELETE FROM [dbo].[MOM_Staff]
DELETE FROM [dbo].[MOM_MeetingVenue]
DELETE FROM [dbo].[MOM_MeetingType]
DELETE FROM [dbo].[MOM_Department]
GO

-- Reseed Identities
DBCC CHECKIDENT ('[dbo].[MOM_Department]', RESEED, 0)
DBCC CHECKIDENT ('[dbo].[MOM_MeetingType]', RESEED, 0)
DBCC CHECKIDENT ('[dbo].[MOM_MeetingVenue]', RESEED, 0)
DBCC CHECKIDENT ('[dbo].[MOM_Staff]', RESEED, 0)
DBCC CHECKIDENT ('[dbo].[MOM_Meetings]', RESEED, 0)
DBCC CHECKIDENT ('[dbo].[MOM_MeetingMember]', RESEED, 0)
GO

-- Re-enable Constraints
ALTER TABLE [dbo].[MOM_MeetingMember] CHECK CONSTRAINT ALL
ALTER TABLE [dbo].[MOM_Meetings] CHECK CONSTRAINT ALL
ALTER TABLE [dbo].[MOM_Staff] CHECK CONSTRAINT ALL
GO

-- 2. INSERT DEPARTMENTS
INSERT INTO [dbo].[MOM_Department] ([DepartmentName], [Created], [Modified]) VALUES
('IT Development', GETDATE(), GETDATE()),
('Human Resources', GETDATE(), GETDATE()),
('Finance & Accounts', GETDATE(), GETDATE()),
('Sales & Marketing', GETDATE(), GETDATE()),
('Operations', GETDATE(), GETDATE());
GO

-- 3. INSERT MEETING TYPES
INSERT INTO [dbo].[MOM_MeetingType] ([MeetingTypeName], [Remarks], [Created], [Modified]) VALUES
('Daily Standup', 'Short daily sync', GETDATE(), GETDATE()),
('Sprint Planning', 'Bi-weekly planning', GETDATE(), GETDATE()),
('Client Demo', 'Product showcase', GETDATE(), GETDATE()),
('Board Meeting', 'Quarterly strategic meet', GETDATE(), GETDATE()),
('Code Review', 'Technical detailed review', GETDATE(), GETDATE()),
('HR Interview', 'Candidate evaluation', GETDATE(), GETDATE());
GO

-- 4. INSERT MEETING VENUES
INSERT INTO [dbo].[MOM_MeetingVenue] ([MeetingVenueName], [Created], [Modified]) VALUES
('Conference Room A (Mumbai)', GETDATE(), GETDATE()),
('Board Room (Delhi)', GETDATE(), GETDATE()),
('Training Hall (Bangalore)', GETDATE(), GETDATE()),
('Online - Microsoft Teams', GETDATE(), GETDATE()),
('Online - Zoom', GETDATE(), GETDATE()),
('Cafeteria Meeting Zone', GETDATE(), GETDATE());
GO

-- 5. INSERT STAFF (Indian Origin Names)
-- Dept IDs: 1=IT, 2=HR, 3=Finance, 4=Sales, 5=Ops
INSERT INTO [dbo].[MOM_Staff] ([DepartmentID], [StaffName], [MobileNo], [EmailAddress], [Created], [Modified]) VALUES
(1, 'Rahul Sharma', '9876543210', 'rahul.sharma@example.com', GETDATE(), GETDATE()),
(1, 'Priya Patel', '9876543211', 'priya.patel@example.com', GETDATE(), GETDATE()),
(1, 'Amit Verma', '9876543212', 'amit.verma@example.com', GETDATE(), GETDATE()),
(1, 'Sneha Reddy', '9876543213', 'sneha.reddy@example.com', GETDATE(), GETDATE()),
(2, 'Anjali Gupta', '9876543214', 'anjali.gupta@example.com', GETDATE(), GETDATE()),
(2, 'Vikram Singh', '9876543215', 'vikram.singh@example.com', GETDATE(), GETDATE()),
(3, 'Rohan Mehta', '9876543216', 'rohan.mehta@example.com', GETDATE(), GETDATE()),
(3, 'Kavita Iyer', '9876543217', 'kavita.iyer@example.com', GETDATE(), GETDATE()),
(4, 'Arjun Nair', '9876543218', 'arjun.nair@example.com', GETDATE(), GETDATE()),
(4, 'Pooja Joshi', '9876543219', 'pooja.joshi@example.com', GETDATE(), GETDATE()),
(5, 'Suresh Kumar', '9876543220', 'suresh.kumar@example.com', GETDATE(), GETDATE()),
(5, 'Neha Malhotra', '9876543221', 'neha.malhotra@example.com', GETDATE(), GETDATE());
GO

-- 6. INSERT MEETINGS (Spread over last 6 months for chart data)
-- Venues: 1-6, Types: 1-6, Depts: 1-5
DECLARE @BaseDate DATE = GETDATE();

-- Current Month
INSERT INTO [dbo].[MOM_Meetings] ([MeetingDate], [MeetingVenueID], [MeetingTypeID], [DepartmentID], [MeetingDescription], [IsCancelled], [Created], [Modified]) VALUES
(DATEADD(day, -2, @BaseDate), 1, 1, 1, 'Daily Standup - IT Team', 0, GETDATE(), GETDATE()),
(DATEADD(day, -5, @BaseDate), 4, 3, 4, 'Sales Client Demo - Tata Group', 0, GETDATE(), GETDATE()),
(DATEADD(day, -8, @BaseDate), 2, 4, 1, 'Q3 Strategy Review', 0, GETDATE(), GETDATE());

-- Last Month
INSERT INTO [dbo].[MOM_Meetings] ([MeetingDate], [MeetingVenueID], [MeetingTypeID], [DepartmentID], [MeetingDescription], [IsCancelled], [Created], [Modified]) VALUES
(DATEADD(month, -1, @BaseDate), 5, 2, 1, 'Sprint Planning - Module A', 0, GETDATE(), GETDATE()),
(DATEADD(month, -1, @BaseDate), 1, 5, 1, 'Code Review - Backend API', 0, GETDATE(), GETDATE()),
(DATEADD(month, -1, @BaseDate), 4, 3, 4, 'Client Demo - Reliance', 0, GETDATE(), GETDATE()),
(DATEADD(month, -1, @BaseDate), 2, 6, 2, 'Senior HR Interview', 0, GETDATE(), GETDATE());

-- 2 Months Ago
INSERT INTO [dbo].[MOM_Meetings] ([MeetingDate], [MeetingVenueID], [MeetingTypeID], [DepartmentID], [MeetingDescription], [IsCancelled], [Created], [Modified]) VALUES
(DATEADD(month, -2, @BaseDate), 1, 1, 1, 'Daily Standup', 0, GETDATE(), GETDATE()),
(DATEADD(month, -2, @BaseDate), 3, 1, 5, 'Ops Sync', 0, GETDATE(), GETDATE()),
(DATEADD(month, -2, @BaseDate), 5, 2, 1, 'Sprint Retro', 0, GETDATE(), GETDATE()),
(DATEADD(month, -2, @BaseDate), 2, 4, 1, 'Board Meeting - Emergency', 1, GETDATE(), GETDATE()); -- Cancelled

-- 3 Months Ago
INSERT INTO [dbo].[MOM_Meetings] ([MeetingDate], [MeetingVenueID], [MeetingTypeID], [DepartmentID], [MeetingDescription], [IsCancelled], [Created], [Modified]) VALUES
(DATEADD(month, -3, @BaseDate), 4, 3, 3, 'Audit Review', 0, GETDATE(), GETDATE()),
(DATEADD(month, -3, @BaseDate), 6, 1, 2, 'Team Lunch & Learn', 0, GETDATE(), GETDATE());

-- 4 Months Ago
INSERT INTO [dbo].[MOM_Meetings] ([MeetingDate], [MeetingVenueID], [MeetingTypeID], [DepartmentID], [MeetingDescription], [IsCancelled], [Created], [Modified]) VALUES
(DATEADD(month, -4, @BaseDate), 1, 1, 1, 'Daily Standup', 0, GETDATE(), GETDATE());

-- 5 Months Ago
INSERT INTO [dbo].[MOM_Meetings] ([MeetingDate], [MeetingVenueID], [MeetingTypeID], [DepartmentID], [MeetingDescription], [IsCancelled], [Created], [Modified]) VALUES
(DATEADD(month, -5, @BaseDate), 5, 2, 1, 'Project Kickoff', 0, GETDATE(), GETDATE()),
(DATEADD(month, -5, @BaseDate), 2, 4, 1, 'Yearly Budget Planning', 0, GETDATE(), GETDATE());
GO

-- 7. INSERT MEETING MEMBERS (Assign staff to meetings)
-- Meetings 1-15 created above
-- Staff 1-12

-- Meeting 1 (Daily Standup IT): Rahul, Priya, Amit, Sneha
INSERT INTO [dbo].[MOM_MeetingMember] ([MeetingID], [StaffID], [IsPresent], [Created], [Modified]) VALUES
(1, 1, 1, GETDATE(), GETDATE()),
(1, 2, 1, GETDATE(), GETDATE()),
(1, 3, 0, GETDATE(), GETDATE()), -- Amit Absent
(1, 4, 1, GETDATE(), GETDATE());

-- Meeting 2 (Sales Demo): Arjun (Sales), Rahul (IT Support)
INSERT INTO [dbo].[MOM_MeetingMember] ([MeetingID], [StaffID], [IsPresent], [Created], [Modified]) VALUES
(2, 9, 1, GETDATE(), GETDATE()),
(2, 1, 1, GETDATE(), GETDATE());

-- Meeting 4 (Sprint Planning): Full IT Team + Scrum Master
INSERT INTO [dbo].[MOM_MeetingMember] ([MeetingID], [StaffID], [IsPresent], [Created], [Modified]) VALUES
(4, 1, 1, GETDATE(), GETDATE()), -- Rahul
(4, 2, 1, GETDATE(), GETDATE()), -- Priya
(4, 3, 1, GETDATE(), GETDATE()), -- Amit
(4, 4, 1, GETDATE(), GETDATE()); -- Sneha

-- Meeting 5 (Code Review): Rahul, Amit
INSERT INTO [dbo].[MOM_MeetingMember] ([MeetingID], [StaffID], [IsPresent], [Created], [Modified]) VALUES
(5, 1, 1, GETDATE(), GETDATE()),
(5, 3, 1, GETDATE(), GETDATE());

-- meeting 6 (Client Demo): Arjun, Pooja
INSERT INTO [dbo].[MOM_MeetingMember] ([MeetingID], [StaffID], [IsPresent], [Created], [Modified]) VALUES
(6, 9, 1, GETDATE(), GETDATE()),
(6, 10, 1, GETDATE(), GETDATE());

-- Random others to boost counts for top staff
INSERT INTO [dbo].[MOM_MeetingMember] ([MeetingID], [StaffID], [IsPresent], [Created], [Modified]) VALUES
(7, 5, 1, GETDATE(), GETDATE()), -- Anjali HR
(8, 11, 1, GETDATE(), GETDATE()), -- Suresh Ops
(9, 1, 1, GETDATE(), GETDATE()), -- Rahul (Very active)
(10, 2, 1, GETDATE(), GETDATE()), -- Priya
(11, 1, 1, GETDATE(), GETDATE()); -- Rahul again
GO
