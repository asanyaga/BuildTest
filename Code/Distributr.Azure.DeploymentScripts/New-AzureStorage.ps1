[CmdletBinding(PositionalBinding=$True)]
Param(
    [Parameter(Mandatory = $true)]
    [String]$Name,    
    [String]$Location = "North Europe"
)

$Name = $Name.ToLower()

# Create a new storage account
Write-Verbose "[Start] creating $Name storage account $Location location"

$storageAcct = New-AzureStorageAccount -StorageAccountName $Name -Location $Location -Verbose
if ($storageAcct)
{
    Write-Verbose "[Finish] creating $Name storage account in $Location location"
}
else
{
    throw "Failed to create a Windows Azure storage account. Failure in New-AzureStorage.ps1"
}

# Get the access key of the storage account
$key = Get-AzureStorageKey -StorageAccountName $Name
if (!$key) {throw "Failed to get storage key for $Name storage account. Failure in Get-AzureStorageKey in New-AzureStorage.ps1"}
$primaryKey = $key.Primary

# Generate the connection string of the storage account
$connectionString = "BlobEndpoint=http://$Name.blob.core.windows.net/;QueueEndpoint=http://$Name.queue.core.windows.net/;TableEndpoint=http://$Name.table.core.windows.net/;AccountName=$Name;AccountKey=$primaryKey"
Return @{AccountName = $Name; AccessKey = $primaryKey; ConnectionString = $connectionString}