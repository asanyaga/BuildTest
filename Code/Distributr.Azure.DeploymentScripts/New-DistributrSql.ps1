[CmdletBinding(PositionalBinding=$True)]

Param
      (
        [parameter(Mandatory=$False)]
        [String] $AppDatabaseName = "appdb",

        [parameter(Mandatory=$False)]
        [String] $UserName = "dbuser",

        # Required
        [parameter(Mandatory=$True)]
        [String] $Password,
        
        [parameter(Mandatory=$False)]
        [String] $StartIPAddress,
        
        [parameter(Mandatory=$False)]
        [String]$EndIPAddress,
        
        [parameter(Mandatory=$False)]
        [String]$Location = "North Europe"     
      )


# Begin - Helper functions --------------------------------------------------------------------------------------------------------------------------



# Create a PSCrendential object from plain text password.
# The PS Credential object will be used to create a database context, which will be used to create database.
Function New-PSCredentialFromPlainText
{
    Param(
        [String]$UserName,
        [String]$Password
    )

    $securePassword = ConvertTo-SecureString -String $Password -AsPlainText -Force

    Return New-Object System.Management.Automation.PSCredential($UserName, $securePassword)
}

# Generate connection string of a given SQL Azure database
Function Get-SQLAzureDatabaseConnectionString
{
    Param(
        [String]$DatabaseServerName,
        [String]$DatabaseName,
        [String]$UserName,
        [String]$Password
    )

    Return "Server=tcp:$DatabaseServerName.database.windows.net,1433;Database=$DatabaseName;User ID=$UserName@$DatabaseServerName;Password=$Password;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;"
}


# End - Helper functions --------------------------------------------------------------------------------------------------------------------------

# Begin - Actual script ---------------------------------------------------------------------------------------------------------------------------
$VerbosePreference = "Continue"
$ErrorActionPreference = "Stop"

Write-Verbose "Get a list of existing SQL Azure database servers"
$servers = Get-AzureSqlDatabaseServer
$databaseServerName = ''
$isNewServer = $servers.Length -eq 0
if($isNewServer) {
    Write-Verbose "[Start] creating SQL Azure database server in $Location location with username $UserName and password $Password"
    $databaseServer = New-AzureSqlDatabaseServer -AdministratorLogin $UserName -AdministratorLoginPassword $Password -Location $Location
    if (!$databaseServer) {throw "Did not create database server. Failure in New-AzureSqlDatabaseServer in New-AzureSql.ps1"}
    $databaseServerName = $databaseServer.ServerName
    Write-Verbose "[Finish] creating SQL Azure database server $databaseServerName in location $Location with username $UserName and password $Password"
}
else
{
    $databaseServerName = $servers[0].ServerName
}


# Apply Firewall Rules
$clientFirewallRuleName = "ClientIPAddress_Main" + [Guid]::NewGuid().ToString().Substring(0,8)
Write-Verbose "Creating client firewall rule '$clientFirewallRuleName'."
New-AzureSqlDatabaseServerFirewallRule -ServerName $databaseServerName `
    -RuleName $clientFirewallRuleName -StartIpAddress $StartIPAddress -EndIpAddress $EndIPAddress | Out-Null  
if($isNewServer){
    $azureFirewallRuleName = "AzureServices"
    Write-Verbose "Creating Azure Services firewall rule '$azureFirewallRuleName'."
    New-AzureSqlDatabaseServerFirewallRule -ServerName $databaseServerName `
        -RuleName $azureFirewallRuleName -StartIpAddress "0.0.0.0" -EndIpAddress "0.0.0.0"
}

$credential = New-PSCredentialFromPlainText -UserName $UserName -Password $Password
if (!$credential) {throw "Failed to create secure credentials. Failure in New-PSCredentialFromPlainText function in New-AzureSql.ps1"}

$context = New-AzureSqlDatabaseServerContext -ServerName $databaseServerName -Credential $credential
if (!$context) {throw "Failed to create db server context for $databaseServerName. Failure in call to New-AzureSqlDatabaseServerContext in New-AzureSql.ps1"}

# Use the database context to create app database
Write-Verbose "[Start] creating database  $AppDatabaseName in database server $databaseServerName"
$appdb = New-AzureSqlDatabase -DatabaseName $AppDatabaseName -Context $context -Verbose
if (!$appdb) {throw "Failed to create $AppDatabaseName application database. Failure in New-AzureSqlDatabase in New-AzureSql.ps1"}
Write-Verbose "[Finish] creating database $AppDatabaseName in database server $databaseServerName"

Write-Verbose "Creating database connection string for $appDatabaseName in database server $databaseServerName"
$appDatabaseConnectionString = Get-SQLAzureDatabaseConnectionString -DatabaseServerName $databaseServerName -DatabaseName $AppDatabaseName -UserName $UserName -Password $Password
if (!$appDatabaseConnectionString) {throw "Failed to create application database connection string for $AppDatabaseName. Failure in Get-SQLAzureDatabaseConnectionString function in New-AzureSql.ps1"}

Write-Verbose "Creating hash table to return..."
$ht = @{ Server = $databaseServerName; UserName = $UserName; Password = $Password; DatabaseName = $AppDatabaseName; ConnectionString = $appDatabaseConnectionString }

Return $ht

# End - Actual script -----------------------------------------------------------------------------------------------------------------------------