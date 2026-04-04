-- 1. Bảng Provinces: Lưu danh sách các tỉnh/thành phố
CREATE TABLE [dbo].[Provinces]
(
	[ProvinceName] [nvarchar](255) NOT NULL PRIMARY KEY
) 
GO
-- 2. Bảng Suppliers: Lưu danh sách nhà cung cấp
CREATE TABLE [dbo].[Suppliers]
(
	[SupplierID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[SupplierName] [nvarchar](255) NOT NULL,
	[ContactName] [nvarchar](255) NOT NULL,
	[Province] [nvarchar](255) NULL,
	[Address] [nvarchar](255) NULL,
	[Phone] [nvarchar](255) NULL,
	[Email] [nvarchar](255) NULL
)
GO
-- 3. Bảng Customers: Lưu danh sách khách hàng
CREATE TABLE [dbo].[Customers]
(
	[CustomerID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[CustomerName] [nvarchar](255) NOT NULL,
	[ContactName] [nvarchar](255) NOT NULL,
	[Province] [nvarchar](255) NULL,
	[Address] [nvarchar](255) NULL,
	[Phone] [nvarchar](255) NULL,
	[Email] [nvarchar](50) NULL,
	[Password] [nvarchar](50) NULL,
	[IsLocked] [bit] NULL
)
GO

-- 4. Bảng Employees: Lưu dữ liệu nhân viên
CREATE TABLE [dbo].[Employees]
(
	[EmployeeID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[FullName] [nvarchar](255) NOT NULL,
	[BirthDate] [date] NULL,
	[Address] [nvarchar](255) NULL,
	[Phone] [nvarchar](255) NULL,
	[Email] [nvarchar](50) NULL UNIQUE,
	[Password] [nvarchar](50) NULL,
	[Photo] [nvarchar](255) NULL,
	[IsWorking] [bit] NULL,
	[RoleNames] [nvarchar](500) NULL
)
GO

-- 5. Bảng Shippers: Lưu dữ liệu người giao hàng
CREATE TABLE [dbo].[Shippers]
(
	[ShipperID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[ShipperName] [nvarchar](255) NOT NULL,
	[Phone] [nvarchar](255) NULL
)
GO

-- 6. Bảng Categories: Lưu danh mục loại hàng
CREATE TABLE [dbo].[Categories]
(
	[CategoryID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[CategoryName] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](255) NULL
)
GO

-- 7. Bảng Products: Lưu dữ liệu mặt hàng
CREATE TABLE [dbo].[Products]
(
	[ProductID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[ProductName] [nvarchar](255) NOT NULL,
	[ProductDescription] [nvarchar](2000) NULL,
	[SupplierID] [int] NULL,
	[CategoryID] [int] NULL,
	[Unit] [nvarchar](255) NOT NULL,
	[Price] [money] NOT NULL,
	[Photo] [nvarchar](255) NULL,
	[IsSelling] [bit] NULL
)
GO

-- 8. Bảng ProductAttributes: Lưu danh sách các thuộc tính của mặt hàng
CREATE TABLE [dbo].[ProductAttributes]
(
	[AttributeID] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[ProductID] [int] NOT NULL,
	[AttributeName] [nvarchar](255) NOT NULL,
	[AttributeValue] [nvarchar](500) NOT NULL,
	[DisplayOrder] [int] NOT NULL
)
GO

-- 9. Bảng ProductPhotos: Lưu danh sách ảnh của mặt hàng
CREATE TABLE [dbo].[ProductPhotos]
(
	[PhotoID] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[ProductID] [int] NOT NULL,
	[Photo] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](255) NOT NULL,
	[DisplayOrder] [int] NOT NULL,
	[IsHidden] [bit] NOT NULL
)
GO

-- 10. Bảng OrderStatus: Lưu dữ liệu định nghĩa các trạng thái của đơn hàng
CREATE TABLE [dbo].[OrderStatus]
(
	[Status] [int] NOT NULL PRIMARY KEY,
	[Description] [nvarchar](50) NOT NULL
)
GO

-- 11. Bảng Orders: Lưu dữ liệu đơn hangf
CREATE TABLE [dbo].[Orders]
(
	[OrderID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[CustomerID] [int] NULL,
	[OrderTime] [datetime] NOT NULL,
	[DeliveryProvince] [nvarchar](255) NULL,
	[DeliveryAddress] [nvarchar](255) NULL,
	[EmployeeID] [int] NULL,
	[AcceptTime] [datetime] NULL,
	[ShipperID] [int] NULL,
	[ShippedTime] [datetime] NULL,
	[FinishedTime] [datetime] NULL,
	[Status] [int] NOT NULL	
)
GO

-- 12. Bảng OrderDetails: Lưu thông tin chi tiết các mặt hàng được bán trong đơn hàng
CREATE TABLE [dbo].[OrderDetails]
(
	[OrderID] [int] NOT NULL,
	[ProductID] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[SalePrice] [money] NOT NULL,
	PRIMARY KEY ([OrderID], [ProductID])
)
GO

-- Thiết lập mối quan hệ giữa các bảng
ALTER TABLE [dbo].[Suppliers]  
ADD FOREIGN KEY([Province])
	REFERENCES [dbo].[Provinces] ([ProvinceName])
GO

ALTER TABLE [dbo].[Customers]  
ADD	FOREIGN KEY([Province])
	REFERENCES [dbo].[Provinces] ([ProvinceName])
GO

ALTER TABLE [dbo].[Products]
ADD	FOREIGN KEY([CategoryID])
	REFERENCES [dbo].[Categories] ([CategoryID])
GO

ALTER TABLE [dbo].[Products]  
ADD	FOREIGN KEY([SupplierID])
	REFERENCES [dbo].[Suppliers] ([SupplierID])
GO

ALTER TABLE [dbo].[ProductAttributes] 
ADD	FOREIGN KEY([ProductID])
	REFERENCES [dbo].[Products] ([ProductID])
GO

ALTER TABLE [dbo].[ProductPhotos]
ADD	FOREIGN KEY([ProductID])
	REFERENCES [dbo].[Products] ([ProductID])
GO

ALTER TABLE [dbo].[Orders]  
ADD	FOREIGN KEY([CustomerID])
	REFERENCES [dbo].[Customers] ([CustomerID])
GO

ALTER TABLE [dbo].[Orders]  
ADD FOREIGN KEY([EmployeeID])
	REFERENCES [dbo].[Employees] ([EmployeeID])
GO

ALTER TABLE [dbo].[Orders]
ADD	FOREIGN KEY([ShipperID])
	REFERENCES [dbo].[Shippers] ([ShipperID])
GO

ALTER TABLE [dbo].[Orders]
ADD	FOREIGN KEY([Status])
	REFERENCES [dbo].[OrderStatus] ([Status])
GO

ALTER TABLE [dbo].[OrderDetails]  
ADD	FOREIGN KEY([OrderID])
	REFERENCES [dbo].[Orders] ([OrderID])
GO

ALTER TABLE [dbo].[OrderDetails]  
ADD FOREIGN KEY([ProductID])
	REFERENCES [dbo].[Products] ([ProductID])
GO


