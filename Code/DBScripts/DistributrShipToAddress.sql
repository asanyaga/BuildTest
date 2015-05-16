
	CREATE TABLE [dbo].[tblShipToAddress](
	[Id] [uniqueidentifier] NOT NULL Primary Key,	
	[CostCentreId] [uniqueidentifier] NOT NULL,	
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](250) NULL,
	[PostalAddress] [nvarchar](250) NULL,
	[PhysicalAddress] [nvarchar](250) NULL,
	[Longitude] [decimal](18,4) NULL,
	[Latitude] [decimal](18,4) NULL,
	[IM_DateCreated] [datetime] NOT NULL,
	[IM_DateLastUpdated] [datetime] NOT NULL,
	[IM_Status] int NOT NULL
	);
ALTER TABLE [dbo].[tblShipToAddress] ADD CONSTRAINT ShipToAdress_CostCentre_FK FOREIGN KEY (CostCentreId) REFERENCES tblCostCentre(Id);

alter table [dbo].[tblDocument] add  ShipToAddress varchar(250) null;


foreach (var addressItem in outlet.ShipToAddresses)
                {
                    var address = _ctx.tblShipToAddress.FirstOrDefault(n => n.Id == addressItem.Id);
                    if (address == null)
                    {
                        address = new tblShipToAddress
                                      {
                                          IM_DateCreated = dt,
                                          IM_Status = (int) EntityStatus.Active
                                      };
                        _ctx.tblShipToAddress.AddObject(address);
                    }
                    address.CostCentreId = costCentre.Id;
                    address.Id = addressItem.Id;
                    address.IM_DateLastUpdated = dt;
                    address.IM_Status = address.IM_Status;
                    ccToSave.tblShipToAddress.Add(address);
                }