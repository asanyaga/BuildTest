[CmdletBinding(PositionalBinding=$True)]
Param(
    [Parameter(Mandatory = $true)]
    [ValidatePattern("^[a-z0-9]*$")]
    [String]$Name                         # required    needs to be alphanumeric    
   
    )

#region - Helper functions --------------------------------------------------------------------------------------------------------------------------


#endregion - Helper funtions -----------------------------------------------------------------------------------------------------------------------------


#region - Actual script -----------------------------------------------------------------------------------------------------------------------------
#include deploy settings
. .\deploysettings.ps1
# Set the output level to verbose and make the script stop on error
$VerbosePreference = "Continue"
$ErrorActionPreference = "Stop"

# Get the time that script execution starts
$startTime = Get-Date
Write-Verbose "Verifying that Windows Azure credentials in the Windows PowerShell session have not expired."
Get-AzureWebsite | Out-Null

Write-Verbose "[Start] removing azure installation: $Name"
$deploySettings = Get-DeploySettings -Name $Name -StartIPAddress "0.0.0.0"

[Xml]$envXml = Get-Content "$($deploySettings.configPath)\webapi-environment.xml"
$sqlServerName = $envXml.environment.sqlAzure.databaseServerName


Remove-AzureWebsite -Name $deploySettings.NameWebApiWebSite -force
Remove-AzureWebsite -Name $deploySettings.NameHQWebSite -force
Remove-AzureStorageAccount -StorageAccountNam $deploySettings.StorageAccountName
Remove-azuresqldatabase -servername $sqlServerName -databasename $deploySettings.SqlAppDatabaseName -force

#endregion - Actual script ------------------------------------------------------------------------------------------------------------------------------- -