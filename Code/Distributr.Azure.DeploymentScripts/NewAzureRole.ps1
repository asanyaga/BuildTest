<#
.Synopsis
   This scripts take a Package (*.cspkg) and config file (*.cscfg) to create a 
   corporate site on Web Role with Azure SQL application database and Storage 
   Account.
#>
#1. Parameters
Param(
    
    #Cloud services Name
    [Parameter(Mandatory = $true)]
    [String]$Name,            
    
    #Cloud Service location 
    [Parameter(Mandatory = $true)]
    [String]$ServiceLocation,     
    
    #Path to configuration file (*.cscfg)     
    [Parameter(Mandatory = $true)]                             
    [String]$ConfigurationFilePath,   
    
    #PackageFilePath:        Path to Package file (*.cspkg)          
    [Parameter(Mandatory = $true)]                             
    [String]$PackageFilePath
               
)

#region - Helper function --------------------------------
<#2.1 CreateCloudService
.Synopsis
This function create a Cloud Services if this Cloud Service don't exists.

.DESCRIPTION
    This function try to obtain the services using $MyServiceName. If we have
    an exception it is mean the Cloud services don’t exist and create it.
.EXAMPLE
    CreateCloudService  "ServiceName" "ServiceLocation"
#> 
Function CreateCloudService 
{
 Param(
    #Cloud services Name
    [Parameter(Mandatory = $true)]
    [String]$MyServiceName,
    #Cloud service Location 
    [Parameter(Mandatory = $true)]
    [String]$MyServiceLocation     
    )

 try
 {
    $CloudService = Get-AzureService -ServiceName $MyServiceName
    Write-Verbose ("cloud service {0} in location {1} exist!" -f $MyServiceName, $MyServiceLocation)
 }
 catch
 { 
   #Create
   Write-Verbose ("[Start] creating cloud service {0} in location {1}" -f $MyServiceName, $MyServiceLocation)
   New-AzureService -ServiceName $MyServiceName -Location $MyServiceLocation
   Write-Verbose ("[Finish] creating cloud service {0} in location {1}" -f $MyServiceName, $MyServiceLocation)
 }
}

<#2.3 Update-Cscfg
.Synopsis
    This function update Cloud Services configuration file with the Azure SQL and Storage account information
.DESCRIPTION
    It load XML file and looking for “dbApplication” and “Storage” XML TAG with the current Azure SQL and Storage account.
    It save updated configuration in a temporal file. 
.EXAMPLE
    Update-Cscfg  `
            -MyConfigurationFilePath $ConfigurationFilePath  `
            -MySqlConnStr $sql.AppDatabase.ConnectionString `
            -MyStorageConnStr $Storage.ConnectionString
.OUTPUTS
   file Path to temp configuration file updated
#>
Function Update-Cscfg 
{
Param (
    #Path to configuration file (*.cscfg)
    [Parameter(Mandatory = $true)]
    [String]$MyConfigurationFilePath,
    #Azure SQL connection string 
    [Parameter(Mandatory = $true)]
    [String]$MySqlConnStr ,
    #Storage Account connection String 
    [Parameter(Mandatory = $true)]
    [String]$MyStorageConnStr 
)
    # Get content of the project source cscfg file
    [Xml]$cscfgXml = Get-Content $MyConfigurationFilePath
    Foreach ($role in $cscfgXml.ServiceConfiguration.Role)
    {
        Foreach ($setting in $role.ConfigurationSettings.Setting)
        {
            Switch ($setting.name)
            {
                "cokeconnectionstring" {$setting.value =$MySqlConnStr} #AppDatabase
                "StorageConnectionString" {$setting.value = $MyStorageConnStr}  #Storage
                "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionStrin" {$setting.value = $MyStorageConnStr}  #Storage
            }
        }
    }
    #Save the change
    $file = "{0}\ServiceConfiguration.Ready.cscfg" -f $configPath
    $cscfgXml.InnerXml | Out-File -Encoding utf8 $file
    Return $file
}
<# 2.4 DeployPackage
.Synopsis
    It deploy service’s  package with his configuration to a Cloud Services 
.DESCRIPTION
    it function try to obtain the Services deployments by name. If exists this deploy is update. In other case,
     it create a Deploy and does the upload.
.EXAMPLE
   DeployPackage -MyServiceName $ServiceName -MyConfigurationFilePath $NewcscfgFilePath -MyPackageFilePath $PackageFilePath         
#>
Function DeployPackage 
{
Param(
    #Cloud Services name
    [Parameter(Mandatory = $true)]
    [String]$MyServiceName,
    #Path to configuration file (*.cscfg)
    [Parameter(Mandatory = $true)]
    [String]$MyConfigurationFilePath,
    #Path to package file (*.cspkg)
    [Parameter(Mandatory = $true)]
    [String]$MyPackageFilePath
)
    $packageFile = [System.IO.Path]::Combine($ScriptPath, $MyPackageFilePath)
    $packageFile = [System.IO.Path]::GetFullPath($packageFile)
    Write-Verbose ("Using package file {0} " -f $packageFile)
    Try
    {
        Get-AzureDeployment -ServiceName $MyServiceName
        Write-Verbose ("[Start] Deploy Service {0}  exist, Will update" -f $MyServiceName)
        Set-AzureDeployment `
            -ServiceName $MyServiceName `
            -Slot Production `
            -Configuration $MyConfigurationFilePath `
            -Package $MyPackageFilePath `
            -Mode Simultaneous -Upgrade
        Write-Verbose ("[finish] Deploy Service {0}  exist, Will update" -f $MyServiceName)
    }
    Catch
    {
        Write-Verbose ("[Start] Deploy Service {0} does not exist, Will create" -f $MyServiceName)
        New-AzureDeployment -ServiceName $MyServiceName -Slot Production -Configuration $MyConfigurationFilePath -Package $packageFile
        Write-Verbose ("[Finish] Deploy Service {0} don't exist, Will create" -f $MyServiceName)
    }
    
}
<#2.5 WaitRoleInstanceReady
.Synopsis
    it wait all role instance are ready
.DESCRIPTION
    Wait until al instance of Role are ready
.EXAMPLE
  WaitRoleInstanceReady $ServiceName
#>
function WaitRoleInstanceReady 
{
Param(
    #Cloud Services name
    [Parameter(Mandatory = $true)]
    [String]$MyServiceName
)
    Write-Verbose ("[Start] Waiting for Instance Ready")
    do
    {
        $MyDeploy = Get-AzureDeployment -ServiceName $MyServiceName  
        foreach ($Instancia in $MyDeploy.RoleInstanceList)
        {
            $switch=$true
            Write-Verbose ("Instance {0} is in state {1}" -f $Instancia.InstanceName, $Instancia.InstanceStatus )
            if ($Instancia.InstanceStatus -ne "ReadyRole")
            {
                $switch=$false
            }
        }
        if (-Not($switch))
        {
            Write-Verbose ("Waiting Azure Deploy running, it status is {0}" -f $MyDeploy.Status)
            Start-Sleep -s 10
        }
        else
        {
            Write-Verbose ("[Finish] Waiting for Instance Ready")
        }
    }
    until ($switch)
}




#endregion - end helper function ---------------------

#region - main script ---------------------------------------

# 3.0 Same variables tu use in the Script
$VerbosePreference = "Continue"
$ErrorActionPreference = "Stop"
$ServiceName = $Name + "role"

# Get the directory of the current script
$ScriptPath = Split-Path -parent $PSCommandPath
$configPath = $scriptPath + '\' + $Name 
# Mark the start time of the script execution
$StartTime = Get-Date
# Define the names of storage account, SQL Azure database and SQL Azure database server firewall rule
$ServiceName = $ServiceName.ToLower()
$sqlAppDatabaseName = $Name + "_db"

# 3.1 Create a new cloud service?
#creating Windows Azure cloud service environment
Write-Verbose ("[Start] Validating  Windows Azure cloud service environment {0}" -f $ServiceName)
CreateCloudService  $ServiceName $ServiceLocation

Write-Verbose ("[Finish] creating Windows Azure cloud service environment {0}" -f $ServiceName)

Write-Verbose "Read from environment file for settings"
# Read from website-environment.xml to get the environment name
[Xml]$envXml = Get-Content $configPath\webapi-environment.xml
if (!$envXml) {throw "Error: Cannot find the webapi-environment.xml in $configPath"}
#build the connections string for cscfg
$cokeconnectionstringtemplate = "metadata=res://*/EF.CokeData.csdl|res://*/EF.CokeData.ssdl|res://*/EF.CokeData.msl;provider=System.Data.SqlClient;provider connection string='data source=tcp:{0}.database.windows.net,1433;initial catalog={1};persist security info=True;user id={2};password={3};multipleactiveresultsets=True;App=EntityFramework'";
$cokeconnectionstring = $cokeconnectionstringtemplate -f $envXml.environment.sqlAzure.databaseServerName, $sqlAppDatabaseName,$envXml.environment.sqlAzure.userName, $envXml.environment.sqlAzure.password
Write-Verbose ("Generated connection string {0}" -f $cokeconnectionstring)
$storageconnectionstringtemplate = "DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}"
$storageconnectionstring = $storageconnectionstringtemplate -f $envXml.environment.storage.accountName,$envXml.environment.storage.accessKey

# 3.4 Upgrade configuration  File with the SQL and Storage references
Write-Verbose "Call update config file"
$NewcscfgFilePath = Update-Cscfg  `
            -MyConfigurationFilePath $ConfigurationFilePath  `
            -MySqlConnStr $cokeconnectionstring `
            -MyStorageConnStr $storageconnectionstring
Write-Verbose ("New Config File {0}" -f $NewcscfgFilePath)

#need to set a default storage account
Write-Verbose "set current storage account name"
$currentSubscription = Get-AzureSubscription -current
Set-AzureSubscription $currentSubscription.SubscriptionName -CurrentStorageAccountName $envXml.environment.storage.accountName

# 3.5 Deploy Package
Write-Verbose "Call deploy package"
DeployPackage -MyServiceName $ServiceName -MyConfigurationFilePath $NewcscfgFilePath -MyPackageFilePath $PackageFilePath

#3.5.1 Delete temporal configFile
#Remove-Item $NewcscfgFilePath

# 3.6 Wait Role isntances Ready
Write-Verbose "Awaiting role"
WaitRoleInstanceReady $ServiceName

#4.1 Mark the finish time of the script execution
#    Output the time consumed in seconds
$finishTime = Get-Date

Write-Host ("Total time used (seconds): {0}" -f ($finishTime - $StartTime).TotalSeconds)

#4.2 Launch the Site
Start-Process -FilePath ("http://{0}.cloudapp.net" -f $ServiceName)

#endregion - main script ------------------------------