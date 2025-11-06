CREATE TABLE ScholarshipPrograms (
    ProgramID AUTOINCREMENT PRIMARY KEY,
    ProgramName TEXT(255),
    Sponsor TEXT(255),
    AmountPerRelease CURRENCY,
    CreatedAt DATETIME,
    UpdatedAt DATETIME
);

CREATE TABLE Grantees (
    GranteeID AUTOINCREMENT PRIMARY KEY,
    ProgramID LONG,
    StudentFullName TEXT(255),
    School TEXT(255),
    Course TEXT(255),
    Status TEXT(50),
    CreatedAt DATETIME,
    UpdatedAt DATETIME,
    FOREIGN KEY (ProgramID) REFERENCES ScholarshipPrograms(ProgramID)
);

CREATE TABLE StipendReleases (
    ReleaseID AUTOINCREMENT PRIMARY KEY,
    GranteeID LONG,
    ReleaseDate DATETIME,
    Amount CURRENCY,
    ORNo TEXT(50),
    Notes MEMO,
    CreatedAt DATETIME,
    UpdatedAt DATETIME,
    FOREIGN KEY (GranteeID) REFERENCES Grantees(GranteeID)
);
