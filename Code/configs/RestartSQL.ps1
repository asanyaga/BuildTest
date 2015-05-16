$service = get-service 'MSSQL$SQLEXPRESS64' -ErrorAction SilentlyContinue
if( $service.status -eq "Running" )
{
    
    "Stopping Dependent Services..."
    $depServices = get-service $service.name -dependentservices | Where-Object {$_.Status -eq "Running"}
    if( $depServices -ne $null )
    {
        foreach($depService in $depServices)
        {
        stop-service $depService.name
        }
    }
    "Stopping Service..."
    stop-service $service.name -force
    "Service Stopped"
}
elseif ( $service.status -eq "Stopped" )
{
"Starting Service..."
start-service $service.name
"Service Started"
}
else
{
"The specified service does not exist"
}