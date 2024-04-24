using Microsoft.Data.SqlClient;

namespace RiccioneDisco.Models
{
    public class DB
    {
        public static string connectionString = "Data Source=SQL6031.site4now.net;Initial Catalog=db_aa6c92_riccionedisco;User Id=db_aa6c92_riccionedisco_admin;Password=RiccioneDisco123;MultipleActiveResultSets=True";


        public static SqlConnection conn = new SqlConnection(connectionString);
    }
}
