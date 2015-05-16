
alter table [dbo].[tblSourcingDocument] add  RouteId uniqueidentifier null;
alter table [dbo].[tblSourcingDocument] add  CentreId uniqueidentifier null;

alter table [dbo].[tblCentre] add  RouteId uniqueidentifier null;
ALTER TABLE [dbo].[tblCentre] ADD CONSTRAINT Centre_Route_FK FOREIGN KEY (RouteId) REFERENCES tblRoutes(Id);




