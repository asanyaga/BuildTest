DECLARE @adminGuid UNIQUEIDENTIFIER
SELECT TOP(1) @adminGuid = id FROM tblUserGroup WHERE name ='Admin'

IF NOT EXISTS(select RoleId from tblUserGroupRoles where RoleId=200 )
BEGIN
Insert into tblUserGroupRoles 
  values  (NEWID(),200,@adminGuid,1,GETDATE(),GETDATE(),1)
END

IF NOT EXISTS(select RoleId from tblUserGroupRoles where RoleId=201 )
BEGIN
Insert into tblUserGroupRoles 
  values  (NEWID(),201,@adminGuid,1,GETDATE(),GETDATE(),1)
END

IF NOT EXISTS(select RoleId from tblUserGroupRoles where RoleId=202 )
BEGIN
Insert into tblUserGroupRoles 
  values  (NEWID(),202,@adminGuid,1,GETDATE(),GETDATE(),1)
END

IF NOT EXISTS(select RoleId from tblUserGroupRoles where RoleId=203 )
BEGIN
Insert into tblUserGroupRoles 
  values  (NEWID(),203,@adminGuid,1,GETDATE(),GETDATE(),1)
END

IF NOT EXISTS(select RoleId from tblUserGroupRoles where RoleId=204 )
BEGIN
Insert into tblUserGroupRoles 
  values  (NEWID(),204,@adminGuid,1,GETDATE(),GETDATE(),1)
END

IF NOT EXISTS(select RoleId from tblUserGroupRoles where RoleId=205 )
BEGIN
Insert into tblUserGroupRoles 
  values  (NEWID(),205,@adminGuid,1,GETDATE(),GETDATE(),1)
END

IF NOT EXISTS(select RoleId from tblUserGroupRoles where RoleId=206 )
BEGIN
Insert into tblUserGroupRoles 
  values  (NEWID(),206,@adminGuid,1,GETDATE(),GETDATE(),1)
END

IF NOT EXISTS(select RoleId from tblUserGroupRoles where RoleId=207 )
BEGIN
Insert into tblUserGroupRoles 
  values  (NEWID(),207,@adminGuid,1,GETDATE(),GETDATE(),1)
END

IF NOT EXISTS(select RoleId from tblUserGroupRoles where RoleId=208 )
BEGIN
Insert into tblUserGroupRoles 
  values  (NEWID(),208,@adminGuid,1,GETDATE(),GETDATE(),1)
END
