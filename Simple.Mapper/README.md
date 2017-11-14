# **Simple.Mapper**

> Mapeador **ADO.NET**, utilizando **MySQL** ou **SQL Server**
Banco de Dados: **TEST**
Tabelas: **Marcas e Carros** executando um exemplo com **FOREIGN_KEY**.

### **Create database**
```
create database [test]
```
### **Create tables**
```
use [test]
go
drop table [dbo].[carros]
drop table [dbo].[marcas]
go
create table [dbo].[marcas](
	[id] [int] identity(1,1) not null,
	[nome] [varchar](max) not null,	
	[ativo] [bit] null default 0,
	constraint [pk_marcas_1] primary key([id])
)
go
create table [dbo].[carros](
	[id] [int] identity(1,1) not null,
	[id_marca] [int] not null,
	[nome] [varchar](max) not null,
	[data] [datetime] not null,
	[ativo] [bit] null default 0,
	constraint [pk_carros_1] primary key([id]),
	constraint [fk_carros_marcas_1] foreign key([id_marca]) references [dbo].[marcas]([id])	
)
```