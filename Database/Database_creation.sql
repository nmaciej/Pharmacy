CREATE TABLE Medicines(
Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
Manufacturer NVARCHAR(40) NOT NULL,
[Name] NVARCHAR(40) NOT NULL UNIQUE,
Price MONEY NOT NULL,
Amount INT NOT NULL,
Prescription BIT NOT NULL
);

CREATE TABLE Orders(
Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
[Date] DATETIME NOT NULL,
Cost MONEY NOT NULL UNIQUE
);

CREATE TABLE MedicinesOrders(
Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
IdOrder INT NOT NULL,
IdMedicine INT NOT NULL,
MedicineAmount INT NOT NULL,
PrescriptionNumber varchar(7)
);

ALTER TABLE MedicinesOrders
ADD CONSTRAINT KF_Orders
FOREIGN KEY (IdOrder)
REFERENCES Orders(Id)

ALTER TABLE MedicinesOrders
ADD CONSTRAINT KF_Medicine
FOREIGN KEY (IdMedicine)
REFERENCES Medicines(Id)

insert into Medicines (Manufacturer, [Name],Price,Amount,Prescription)
values ('Polpharma', 'Ascodan', 19.99, 20, 0),
		('Polpharma', 'Scorbolamid', 9.99, 15, 0),
		('Polpharma', 'Ranigast', 4.49, 27, 0),
		('Polpharma', 'Etopiryna',7.99, 6, 0),
		('Polpharma', 'Polprazol', 39.99, 20, 1),
		('Polpharma', 'Starazolin', 15.99, 20, 0)

insert into Medicines (Manufacturer, [Name],Price,Amount,Prescription)
values ('Polski Lek', 'femiMag', 13.99, 70, 1),
		('Polski Lek', 'Plusssz', 17.99, 55, 0),
		('Polski Lek', 'bobolen', 8.99, 13, 1),
		('Polski Lek', 'Actiferol', 4.99, 20, 0)