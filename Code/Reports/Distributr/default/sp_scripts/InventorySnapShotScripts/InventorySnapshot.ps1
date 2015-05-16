$connString = "Data Source=10.0.0.251;Initial Catalog=hq;User Id=sa; Password=P@ssw0rd;"

$connection = new-object System.Data.SqlClient.SqlConnection $connString

$connection.Open()

$Command = new-Object System.Data.SqlClient.SqlCommand("usp_tblInventoryDailySnapshot", $connection)

$Command.CommandType = [System.Data.CommandType]'StoredProcedure'

$Command.ExecuteNonQuery() | Out-Null

$connection.Close() | Out-Null

$Command.Dispose() | Out-Null

$connection.Dispose() | Out-Null






















