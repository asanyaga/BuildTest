[CmdletBinding(PositionalBinding=$True)]
Param(
    [Parameter(Mandatory = $true)]
    [ValidatePattern("^[a-z0-9]*$")]
    [String]$Name,                             # required    needs to be alphanumeric  
    [System.Collections.Hashtable] $deploySettings 
      )

#region - Helper functions --------------------------------------------------------------------------------------------------------------------------
Function New-EnvironmentXml
{
    Param(
        [String]$EnvironmentName,
        [String]$WebsiteName,
        [System.Collections.Hashtable]$Storage,
        [System.Collections.Hashtable]$Sql,
        [String]$filename,
        [System.Collections.Hashtable] $deploySettings

    )

    $templateFile = "$($deploySettings.ScriptPath)\website-environment.template"
    Write-Verbose $templateFile
    
    [String]$template = Get-Content $templateFile 
    
    $xml = $template -f $EnvironmentName, $WebsiteName, `
                        $Storage.AccountName, $Storage.AccessKey, $Storage.ConnectionString, `
                        ([String]$Sql.Server).Trim(), $Sql.UserName, $Sql.Password, `
                        $Sql.ConnectionString
    
    $xml | Out-File -Encoding utf8 -FilePath "$($deploySettings.ConfigPath)\$filename"
}

Function New-PublishXml
{
    Param(
        [Parameter(Mandatory = $true)]
        [String]$WebsiteName,
        [System.Collections.Hashtable] $deploySettings

    )
    
    # Get the current subscription
    $s = Get-AzureSubscription -Current
    if (!$s) {throw "Cannot get Windows Azure subscription. Failure in Get-AzureSubscription in New-PublishXml in New-AzureWebsiteEnv.ps1"}

    $thumbprint = $s.Certificate.Thumbprint
    if (!$thumbprint) {throw "Cannot get subscription cert thumbprint. Failure in Get-AzureSubscription in New-PublishXml in New-AzureWebsiteEnv.ps1"}
    
    # Get the certificate of the current subscription from your local cert store
    $cert = Get-ChildItem Cert:\CurrentUser\My\$thumbprint
    if (!$cert) {throw "Cannot find subscription cert in Cert: drive. Failure in New-PublishXml in New-AzureWebsiteEnv.ps1"}

    $website = Get-AzureWebsite -Name $WebsiteName
    if (!$website) {throw "Cannot get Windows Azure website: $WebsiteName. Failure in Get-AzureWebsite in New-PublishXml in New-AzureWebsiteEnv.ps1"}
    
    # Compose the REST API URI from which you will get the publish settings info
    $uri = "https://management.core.windows.net:8443/{0}/services/WebSpaces/{1}/sites/{2}/publishxml" -f `
        $s.SubscriptionId, $website.WebSpace, $Website.Name

    # Get the publish settings info from the REST API
    $publishSettings = Invoke-RestMethod -Uri $uri -Certificate $cert -Headers @{"x-ms-version" = "2013-06-01"}
    if (!$publishSettings) {throw "Cannot get Windows Azure website publishSettings. Failure in Invoke-RestMethod in New-PublishXml in New-AzureWebsiteEnv.ps1"}

    # Save the publish settings info into a .publishsettings file
    # and read the content as xml
    $cp = $deploySettings.ConfigPath
    $publishSettings.InnerXml >  $cp\$WebsiteName.publishsettings
    [Xml]$xml = Get-Content "$($deploySettings.ConfigPath)\$WebsiteName.publishsettings"
    if (!$xml) {throw "Cannot get website publishSettings XML for $WebsiteName website. Failure in Get-Content in New-PublishXml in New-AzureWebsiteEnv.ps1"}

    # Get the publish xml template and generate the .pubxml file
    [String]$template = Get-Content "$($deploySettings.ScriptPath)\pubxml.template"
    ($template -f $website.HostNames[0], $xml.publishData.publishProfile.publishUrl.Get(0), $WebsiteName) `
        | Out-File -Encoding utf8 ("{0}\{1}.pubxml" -f $deploySettings.ConfigPath, $WebsiteName)
}

Function CreateWebsite
{
     Param(
        [System.Collections.Hashtable]$Storage,
        [System.Collections.Hashtable]$Sql,
        [String]$NameWebSite,
        
        [String]$EnvironmentXmlFile,
        [System.Collections.Hashtable] $deploySettings

    )
    Write-Verbose "Creating a Windows Azure website: $NameWebSite"
    # Create a new website
    #    The New-AzureWebsite cmdlet is exported by the Azure module.
    $website = New-AzureWebsite -Name $NameWebSite -Location $deploySettings.Location -Verbose
    if (!$website) {throw "Error: Website was not created. Terminating the script unsuccessfully. Fix the errors that New-AzureWebsite returned and try again."}

    Write-Verbose "[Start] Adding settings to website: $NameWebSite"
    # Configure app settings for storage account
    $cokeconnectionstringtemplate = "metadata=res://*/EF.CokeData.csdl|res://*/EF.CokeData.ssdl|res://*/EF.CokeData.msl;provider=System.Data.SqlClient;provider connection string='data source=tcp:{0}.database.windows.net,1433;initial catalog={1};persist security info=True;user id={2};password={3};multipleactiveresultsets=True;App=EntityFramework'";
    $cokeconnectionstring = $cokeconnectionstringtemplate -f $sql.Server, $sql.DatabaseName,$sql.UserName, $sql.Password
    $storageconnectionstringtemplate = "DefaultEndpointsProtocol=http;AccountName={0};AccountKey={1}"
    $storageconnectionstring = $storageconnectionstringtemplate -f $deploySettings.StorageAccountName, $storage.AccessKey

    $appSettings = @{ `
        "StorageAccountName" = $deploySettings.StorageAccountName; `
        "StorageAccountAccessKey" = $storage.AccessKey; `
        "cokeconnectionstring" = $cokeconnectionstring; `
        "StorageConnectionString" = $storageconnectionstring; `
    }

    # Configure connection strings for appdb and ASP.NET member db
    $connectionStrings = ( `
        @{Name = $sqlAppDatabaseName; Type = "SQLAzure"; ConnectionString = $sql.AppDatabase.ConnectionString} `
    )

    Write-Verbose "Adding connection strings and storage account name/key to the new $NameWebSite website."
    # Add the connection string and storage account name/key to the website
    $error.clear()
    Set-AzureWebsite -Name $NameWebSite -AppSettings $appSettings #-ConnectionStrings $connectionStrings
    if ($error) {throw "Error: Call to Set-AzureWebsite with config settings failed."}

    Write-Verbose "[Finish] Adding settings to web api website: $NameWebSite"
    Write-Verbose "[Finish] creating Windows Azure environment: $NameWebSite"
    
    #-- Write the environment info to an xml file so that the deploy script can consume
    Write-Verbose "[Begin] writing environment info to webapi-environment.xml"
    New-EnvironmentXml -EnvironmentName $NameWebSite -WebsiteName $NameWebSite -Storage $storage -Sql $sql -filename $EnvironmentXmlFile -deploySettings $deploySettings

    if (!(Test-path "$($deploySettings.ConfigPath)\$EnvironmentXmlFile"))
    {
        throw "The script did not generate a webapi-environment.xml file that is required to deploy the website." 
    }
    else 
    {
        Write-Verbose "$($deploySettings.ConfigPath)\$EnvironmentXmlFile"
        Write-Verbose "[Finish] writing environment info to webapi-environment.xml"
    }

    #-- Generate the .pubxml file which will be used by webdeploy later
    Write-Verbose "[Begin] generating $NameWebApiWebSite.pubxml file"
    New-PublishXml -Website $NameWebSite -deploySettings $deploySettings
    
    if (!(Test-path "$($deploySettings.ConfigPath)\$NameWebSite.pubxml"))
    {
        throw "The script did not generate a $NameWebSite.pubxml file that is required for deployment. Try to rerun the New-PublishXml function in the New-AzureWebisteEnv.ps1 script."
    }
    else 
    {
        Write-Verbose "$($deploySettings.ConfigPath)\$NameWebSite.pubxml"
        Write-Verbose "[Finish] generating $NameWebSite.pubxml file"
    }

}



#endregion - Helper funtions -----------------------------------------------------------------------------------------------------------------------------


#region - Actual script -----------------------------------------------------------------------------------------------------------------------------

# Set the output level to verbose and make the script stop on error
$VerbosePreference = "Continue"
$ErrorActionPreference = "Stop"

# Get the time that script execution starts
$startTime = Get-Date
Write-Verbose "Verifying that Windows Azure credentials in the Windows PowerShell session have not expired."
Get-AzureWebsite | Out-Null

Write-Verbose "[Start] creating Windows Azure website environment: $Name"


#------------------ CREATE DATABASE

Write-Verbose "Creating a Windows Azure database server and databases"
# -------- Create a SQL Azure database server, app and member databases
$sql1 = & ".\New-DistributrSql.ps1" `
    -AppDatabaseName $deploySettings.SqlAppDatabasename `
    -UserName $deploySettings.SqlDatabaseUserName `
    -Password $deploySettings.SqlDatabasePassword `
    -StartIPAddress $deploySettings.StartIPAddress `
    -EndIPAddress $deploySettings.EndIPAddress `
    -Location $deploySettings.Location

if (!$sql1) {throw "Error: The database server or databases were not created. Terminating the script unsuccessfully. Failures occurred in New-AzureSql.ps1."}
$sql = @{ Server = $sql1.Server; UserName = $sql1.UserName; Password = $sql1.Password; DatabaseName = $sql1.DatabaseName; ConnectionString = $sql1.ConnectionString }

$sqlserver = "tcp:" + $sql.Server + ".database.windows.net,1433"

#----------- RUN SCRIPTS AGAINST DATABASE
Write-Verbose "Running script against newly created database"
#Invoke-SqlCmd -inputfile ".\sqlscript\azuredbsetup.sql" -database $sql.DatabaseName -password $sql.Password -ServerInstance $sqlserver -username $sql.UserName
Write-Verbose "Completed running sql script"
Set-Location $deploySettings.ScriptPath
#------------- CREATE STORAGE ACCOUNT
Write-Verbose "Creating a Windows Azure storage account: $($deploySettings.StorageAccountName)"
# Create a new storage account
$storage = & ".\New-AzureStorage.ps1" -Name $deploySettings.StorageAccountName -Location $deploySettings.Location
if (!$storage) {throw "Error: Storage account was not created. Terminating the script unsuccessfully. Fix the errors that New-AzureStorage.ps1 script returned and try again."}

#------- CREATE WEBAPI WEBSITE
CreateWebsite -Storage $storage -Sql $sql -NameWebSite $deploySettings.NameWebApiWebSite  -EnvironmentXmlFile $deploySettings.WebApiEnvironmentFile -deploySettings $deploySettings

#------- CREATE HQ WEBSITE
CreateWebsite -Storage $storage -Sql $sql -NameWebSite $deploySettings.NameHQWebSite  -EnvironmentXmlFile $deploySettings.HQEnvironmentFile -deploySettings $deploySettings


Write-Verbose "Script is complete."
# Mark the finish time of the script execution
$finishTime = Get-Date
# Output the time consumed in seconds
$TotalTime = ($finishTime - $startTime).TotalSeconds
Write-Output "Total time used (seconds): $TotalTime"

#endregion - Actual script ------------------------------------------------------------------------------------------------------------------------------- -