
Param(
    [Parameter(Mandatory = $true)]
    [String]$ProjectFile,
    [Switch]$Launch,
    [String]$Name,
    [String]$EnvironmentFile
)

# Begin - Actual script -----------------------------------------------------------------------------------------------------------------------------
 
# Set the output level to verbose and make the script stop on error
$VerbosePreference = "Continue"
$ErrorActionPreference = "Stop"
 
$scriptPath = Split-Path -parent $PSCommandPath
$configPath = $scriptPath + '\' + $Name 

# Verify that the account credentials are current in the Windows 
#  PowerShell session. This call fails if the credentials have
#  expired in the session.
Write-Verbose "Verifying that Windows Azure credentials in the Windows PowerShell session have not expired."
Get-AzureWebsite | Out-Null
 
# Mark the start time of the script execution
$startTime = Get-Date

# Build and publish the project via web deploy package using msbuild.exe 
Write-Verbose ("[Start] deploying to Windows Azure website {0}" -f $websiteName)

# Read from website-environment.xml to get the environment name
[Xml]$envXml = Get-Content "$configPath\$EnvironmentFile"
if (!$envXml) {throw "Error: Cannot find the $EnvironmentFile in $configPath"}
$websiteName = $envXml.environment.name

# Read from the publish settings file to get the deploy password
$publishXmlFile = Join-Path $configPath -ChildPath ($websiteName + ".pubxml")
[Xml]$publishxml = Get-Content "$configPath\$websiteName.pubxml"
if (!$publishxml) {throw "Error: Cannot find a $websiteName.pubxml file for the $website web site in $configPath."}

[Xml]$xml = Get-Content "$configPath\$websiteName.publishsettings"
if (!$xml) {throw "Error: Cannot find a publishsettings file for the $website web site in $configPath."}
$password = $xml.publishData.publishProfile.userPWD[0]
Write-Verbose ("Publish Windows Azure website publish profile path {0}" -f $publishXmlFile)
# Run MSBuild to publish the project
& "$env:windir\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" $ProjectFile `
    /p:DeployOnBuild=true `
    /p:PublishProfile=$publishXmlFile `
    /p:Password=$password `
    /p:Configuration=Release `
    /verbosity:m `
    /p:VisualStudioVersion=11.0

Write-Verbose "[Finish] deploying to Windows Azure website $websiteName"
# Mark the finish time of the script execution
$finishTime = Get-Date

# Output the time consumed in seconds
Write-Output "Total time used (seconds): ($finishTime - $startTime).TotalSeconds)"s

# if -Launch, launch the browser to show the website
If ($Launch)
{
    Show-AzureWebsite -Name $websiteName
}

# End - Actual script -------------------------------------------------------------------------------------------------------------------------------