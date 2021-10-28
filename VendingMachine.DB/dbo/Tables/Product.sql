CREATE TABLE [dbo].[Product] (
    [Id]       INT        IDENTITY (1, 1) NOT NULL,
    [Name]     NCHAR (10) NOT NULL,
    [SellerId] INT        NOT NULL,
    [Amount]   INT        NOT NULL,
    [Cost]     INT        NOT NULL,
    CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Product_User] FOREIGN KEY ([SellerId]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [UQ_Product_Name_Seller] UNIQUE NONCLUSTERED ([Name] ASC, [SellerId] ASC)
);

