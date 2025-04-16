insert into dbo.Categories ( Name, Description)
values ( N'Laptops', N'Tecnologia'),
       ( N'Smartphones', N'Tecnologia'),
       ( N'Audio', N'Sonido'),
       ( N'Wearables', N'Relojs'),
       ( N'Cámaras', N'Tennologia'),
       ( N'Televisores', N'Tennologia'),
       ( N'Gaming', N'Gaming'),
       ( N'Impresoras', N'Tecnologia');

insert into dbo.Products ( Name, Description, Price, Stock, CategoryId, CreatedAt, Status)
values  (N'Producto 1', N'Description 1', 23.40, 24, 2, N'2025-04-15 22:53:40.6260791', 1),
        ( N'Producto 2', N'Description 2', 50.30, 203, 1, N'2025-04-15 22:54:05.4701193', 1),
        ( N'Producto 3', N'Description 3', 74.50, 100, 7, N'2025-04-15 22:55:12.4408840', 1),
        ( N'Producto 4', N'Description 3', 34.67, 100, 3, N'2025-04-15 22:55:12.4408840', 1),
        ( N'Producto 5', N'Description 3', 100.3, 100, 8, N'2025-04-15 22:55:12.4408840', 1),
        ( N'Producto 6', N'Description 3', 34.5, 231, 1, N'2025-04-15 22:55:12.4408840', 1),
        ( N'Producto 7', N'Description 3', 12.4, 31, 2, N'2025-04-15 22:55:12.4408840', 1),
        ( N'Producto 8', N'Description 3', 23.4, 32, 3, N'2025-04-15 22:55:12.4408840', 1),
        ( N'Producto 9', N'Description 3', 54.6, 43, 4, N'2025-04-15 22:55:12.4408840', 1);

insert into dbo.ProductImages ( ProductId, ImageUrl, CreateAt, PublicId)
values  ( 1, N'https://res.cloudinary.com/ddrwuiowu/image/upload/v1744757619/abdae4dd-f57a-4d29-8d17-c0f5963710d7.png', N'2025-04-15 22:53:42.2036355', N'abdae4dd-f57a-4d29-8d17-c0f5963710d7'),
        ( 2, N'https://res.cloudinary.com/ddrwuiowu/image/upload/v1744757643/ecd0cc68-1d3e-4237-9ae8-3b8183647cdc.jpg', N'2025-04-15 22:54:06.7065427', N'ecd0cc68-1d3e-4237-9ae8-3b8183647cdc'),
        ( 3, N'https://res.cloudinary.com/ddrwuiowu/image/upload/v1744757711/61dd6746-143b-468f-87d3-0c3f9d57d759.jpg', N'2025-04-15 22:55:14.6385357', N'61dd6746-143b-468f-87d3-0c3f9d57d759'),
        ( 4, N'https://res.cloudinary.com/ddrwuiowu/image/upload/v1744757711/61dd6746-143b-468f-87d3-0c3f9d57d759.jpg', N'2025-04-15 22:55:14.6385357', N'61dd6746-143b-468f-87d3-0c3f9d57d759'),
        ( 5, N'https://res.cloudinary.com/ddrwuiowu/image/upload/v1744757711/61dd6746-143b-468f-87d3-0c3f9d57d759.jpg', N'2025-04-15 22:55:14.6385357', N'61dd6746-143b-468f-87d3-0c3f9d57d759'),
        ( 6, N'https://res.cloudinary.com/ddrwuiowu/image/upload/v1744757711/61dd6746-143b-468f-87d3-0c3f9d57d759.jpg', N'2025-04-15 22:55:14.6385357', N'61dd6746-143b-468f-87d3-0c3f9d57d759'),
        ( 7, N'https://res.cloudinary.com/ddrwuiowu/image/upload/v1744757711/61dd6746-143b-468f-87d3-0c3f9d57d759.jpg', N'2025-04-15 22:55:14.6385357', N'61dd6746-143b-468f-87d3-0c3f9d57d759'),
        ( 8, N'https://res.cloudinary.com/ddrwuiowu/image/upload/v1744757711/61dd6746-143b-468f-87d3-0c3f9d57d759.jpg', N'2025-04-15 22:55:14.6385357', N'61dd6746-143b-468f-87d3-0c3f9d57d759'),
        ( 9, N'https://res.cloudinary.com/ddrwuiowu/image/upload/v1744757711/61dd6746-143b-468f-87d3-0c3f9d57d759.jpg', N'2025-04-15 22:55:14.6385357', N'61dd6746-143b-468f-87d3-0c3f9d57d759');
