USE [cokeunittests]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_tblInventorySerials_tblDocument]') AND parent_object_id = OBJECT_ID(N'[dbo].[tblInventorySerials]'))
ALTER TABLE [dbo].[tblInventorySerials] DROP CONSTRAINT [FK_tblInventorySerials_tblDocument]
GO


