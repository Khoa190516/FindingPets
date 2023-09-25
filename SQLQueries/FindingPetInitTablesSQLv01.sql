create table UserRole(
ID int not null primary key,
RoleName nvarchar(50) not null
)

create table AuthenUser(
ID uniqueidentifier not null primary key default newid(),
FullName nvarchar(50) not null,
Email nvarchar(50) not null,
Phone varchar(12) not null,
IsActive bit default 0,
UserRole int foreign key references UserRole(ID)  not null,
)



create table Post(
ID uniqueidentifier not null primary key default newid(),
Description nvarchar(max),
Created datetime default getdate(),
Contact nvarchar(max),
IsBanned bit default 0,
IsClosed bit default 0,
OwnerID uniqueidentifier not null foreign key references AuthenUser(ID),
)

create table PostImages(
ID uniqueidentifier not null primary key default newid(),
ImageBase64 nvarchar(max),
PostID uniqueidentifier foreign key references Post(ID) not null,
)

drop table AuthenUser
drop table Post
drop table PostImages