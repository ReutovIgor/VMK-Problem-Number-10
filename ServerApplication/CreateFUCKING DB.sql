CREATE TABLE [dbo].[Man] (
    [Id_Man]      INT        IDENTITY (1, 1) NOT NULL,
    [Name]        NCHAR (20) NULL,
    [Surname]     NCHAR (20) NULL,
    [Patronymic]  NCHAR (20) NULL,
    [DateOfBirth] DATE       NULL,
    [Username]    NCHAR (20) NULL,
    [Password]    NCHAR (20) NULL,
    [Approved]    BIT        NULL,
    PRIMARY KEY CLUSTERED ([Id_Man] ASC)
);

CREATE TABLE [dbo].[Patient] (
	[Id_Patient]	INT        IDENTITY (1, 1) NOT NULL,
	[Id_Man]		INT FOREIGN KEY REFERENCES [Man]([Id_Man]),
	PRIMARY KEY CLUSTERED ([Id_Patient] ASC)
);

CREATE TABLE [dbo].[Manager] (
	[Id_Manager]	INT        IDENTITY (1, 1) NOT NULL,
	[Id_Man]		INT FOREIGN KEY REFERENCES [Man]([Id_Man]),
	PRIMARY KEY CLUSTERED ([Id_Manager] ASC)
);

CREATE TABLE [dbo].[TimeTable] (
	[Id_TimeTable]	INT        IDENTITY (1, 1) NOT NULL,
	[Monday]		nvarchar(100)	NOT NULL,
	[Tuesday]		nvarchar(100)	NOT NULL,
	[Wednesday]		nvarchar(100)	NOT NULL,
	[Thursday]		nvarchar(100)	NOT NULL,
	[Friday]		nvarchar(100)	NOT NULL,
	[Saturday]		nvarchar(100)	NOT NULL,
	[Sunday]		nvarchar(100)	NOT NULL,
	[Vacation]		nvarchar(100)	NOT NULL,
	PRIMARY KEY CLUSTERED ([Id_TimeTable] ASC)
);

CREATE TABLE [dbo].[Specialization] (
	[Id_Specialization]		INT        IDENTITY (1, 1) NOT NULL,
	[Specialization]		nvarchar(50),
	PRIMARY KEY CLUSTERED ([Id_Specialization] ASC)
);

CREATE TABLE [dbo].[Subsidiary] (
	[Id_Subsidiary]		INT        IDENTITY (1, 1) NOT NULL,
	[Address]			nvarchar(100),
	[Phone]				nvarchar(20)
	PRIMARY KEY CLUSTERED ([Id_Subsidiary] ASC)
);

CREATE TABLE [dbo].[Doctor] (
	[Id_Doctor]				INT        IDENTITY (1, 1) NOT NULL,
	[Id_Man]				INT FOREIGN KEY REFERENCES [Man]([Id_Man]),
	[Id_Subsidiary]			INT FOREIGN KEY REFERENCES [Subsidiary]([Id_Subsidiary]),
	[Id_Specialization]		INT FOREIGN KEY REFERENCES [Specialization]([Id_Specialization]),
	[Id_Manager]			INT FOREIGN KEY REFERENCES [Manager]([Id_Manager]),
	[Id_TimeTable]			INT FOREIGN KEY REFERENCES [TimeTable]([Id_TimeTable]),
	PRIMARY KEY CLUSTERED ([Id_Doctor] ASC)
);

CREATE TABLE [dbo].[Consultation] (
	[Id_Consultation]		INT        IDENTITY (1, 1) NOT NULL,
	[Id_Patient]			INT FOREIGN KEY REFERENCES [Patient]([Id_Patient]),
	[Id_Doctor]				INT FOREIGN KEY REFERENCES [Doctor]([Id_Doctor]),
	[ConsultationTime]		datetime NOT NULL,
	[Begin]					time NOT NULL,
	[End]					time NOT NULL,
	[Code]					nvarchar(20),
	[Status]				nvarchar(10),
	[PatientText]			nvarchar(max),
	[DoctorText]			nvarchar(max),
	PRIMARY KEY CLUSTERED ([Id_Consultation] ASC)
);

CREATE TABLE [dbo].[OnLineConsultation] (
	[Id_OnlineConsultation]		INT        IDENTITY (1, 1) NOT NULL,
	[Id_Patient]				INT FOREIGN KEY REFERENCES [Patient]([Id_Patient]),
	[Id_Doctor]					INT FOREIGN KEY REFERENCES [Doctor]([Id_Doctor]),
	PRIMARY KEY CLUSTERED ([Id_OnlineConsultation] ASC)
);

CREATE TABLE [dbo].[Dialog] (
	[Id_Dialog]					INT        IDENTITY (1, 1) NOT NULL,
	[Id_OnlineConsultation]		INT FOREIGN KEY REFERENCES [OnLineConsultation]([Id_OnlineConsultation]),	
	[Author]					nvarchar(30),
	[Message]					nvarchar(max),
	PRIMARY KEY CLUSTERED ([Id_Dialog] ASC)
);

