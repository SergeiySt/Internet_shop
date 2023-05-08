create database db_internet_shop;  

use db_internet_shop;
go


create table Client
(
	id_client int not null identity(1,1) primary key,
	CSurname nvarchar(50) check (CSurname <> '') not null,
	CName nvarchar(50) check (CName  <> '') not null,
	CPobatkovi nvarchar(50) check (CPobatkovi  <> '') not null,
	CEmail nvarchar(50) check (CEmail  <> '') not null,
	LPhone int not null,
    CLogin nvarchar(50) check (CLogin <> '') not null,
	CPassword nvarchar(100) check (CPassword <> '') not null,
);
go


create table Personnel 
(	
	id_personnel int not null identity(1,1) primary key,
	PSurname nvarchar(50) check (PSurname <> '') not null,
	PName nvarchar(50) check (PName  <> '') not null,
	PPobatkovi nvarchar(50) check (PPobatkovi  <> '') not null,
	PLogin nvarchar(50) check (PLogin <> '') not null,
	PPassword nvarchar(100) check (PPassword <> '') not null,
	PRole nvarchar(50) check (PRole <> '') not null
);
go

create table Goods
(	
	id_goods int not null identity(1,1) primary key,
	GName nvarchar(100) check (GName  <> '') not null,
	GType nvarchar(50) check (GType  <> '') not null,
	GBrand nvarchar(50) check (GBrand  <> '') not null,
	GDescription nvarchar(max) check (GDescription  <> '') not null,
	GPrice money not null,
	GCount int not null,
	GPicture varbinary(max ) not null
);
go


create table GoodsOrder 
(
	id_goodsOrder int not null identity(1,1) primary key,
	ONumberOrder int not null,
	ONameGoods nvarchar(100) not null,
	OCount int not null,
	OAdress nvarchar(max) not null,
	OFIO nvarchar(max) not null,
	OPrice money not null,
	OStatus bit not null default 0,
);
go


insert into Client values
('Кириленко', 'Жора', 'Олександрович', '1234@gmail.com', 0999999, 'Zhora24', '12345'),
('Сидоренко', 'Світлана', 'Олександрівна', '3454@gmail.com', 0977777, 'Svitlana34', '123456');
go

insert into Personnel values
('Полонський', 'Віктор', 'Олексадрович', 'Polonsku', '12345', 'admin'),
('Пінчук', 'Оксана', 'Володимирівна', 'Pinchyk', '12345', 'admin');
go