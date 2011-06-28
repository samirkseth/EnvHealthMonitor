param([string]$env, [string] $machine)

Function Execute-Oracle-Sql ($server, $instance, $userid, $pwd, $query)
{
	[Reflection.Assembly]::LoadFile("C:\Setup\ODAC64\odp.net\bin\4\Oracle.DataAccess.dll") 	
	$connection = new-object Oracle.DataAccess.Client.OracleConnection( `
		“Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=$server)(PORT=1521)) `
		(CONNECT_DATA=(SERVICE_NAME=$instance)));User Id=$userid;Password=$pwd;”);
#		$connection = New-Object System.Data.OracleClient.OracleConnection($connectionString) 
	
	$set = new-object system.data.dataset   
	$adapter = new-object Oracle.DataAccess.Client.OracleDataAdapter ($query, $connection) 
	$adapter.Fill($set) 
	$table = new-object system.data.datatable 
	$table = $set.Tables[0]
	#return table
	$table
}
