using System.Data.SqlClient;

var connStr = Environment.GetEnvironmentVariable("LOCAL_SQL_SERVER_CONNSTR", EnvironmentVariableTarget.User);

try
{
    SqlConnection conn = new(connStr);

    conn.Open();

    Console.WriteLine(conn.State);
}
catch (Exception ex)
{
    Console.WriteLine("Error: {0}", ex.ToString());
}