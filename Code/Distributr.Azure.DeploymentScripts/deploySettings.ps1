Function Get-DeploySettings
{
    
    Param(
        [String]$Name,
        [String]$StartIPAddress
     )
     $SqlDatabaseUserName = "tonytest"
     $SqlDatabasePassword = "Doggie01"
     $EndIPAddress = $StartIPAddress
     $Location = 'North Europe'
     # Define the names of website, storage account, SQL Azure database and SQL Azure database server firewall rule
     $Name = $Name.ToLower()
     $NameWebApiWebSite = $Name + "webapi"
     $NameHQWebSite = $Name + "hq"
     $storageAccountName = $Name + "storage"
     $sqlAppDatabaseName = $Name + "_db"
     $sqlDatabaseServerFirewallRuleName = $Name + "rule"
     $scriptPath = Split-Path -parent $PSCommandPath
     $configPath = $ScriptPath + '\' + $Name
     $deploySettings = @{ 
        Name = $Name;
        Location = $Location; 
        ScriptPath = $scriptPath;
        ConfigPath = $configPath;
        NameWebApiWebSite = $NameWebApiWebSite;
        NameHQWebSite = $NameHQWebSite;
        StorageAccountName = $storageAccountName;
        SqlAppDatabasename = $sqlAppDatabaseName;
        SqlDatabaseUserName = $SqlDatabaseUserName;
        SqlDatabasePassword = $SqlDatabasePassword;
        StartIPAddress = $StartIPAddress;
        EndIPAddress = $EndIPAddress;
        SqlDatabaseServerFirewallRuleName = $sqlDatabaseServerFirewallRuleName;
        WebApiEnvironmentFile = "webapi-environment.xml";
        HQEnvironmentFile = "hq-environment.xml"

        }
     return $deploySettings

}
