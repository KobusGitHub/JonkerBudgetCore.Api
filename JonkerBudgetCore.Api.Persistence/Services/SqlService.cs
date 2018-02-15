using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace SGStatus.WebApi.Persistence.Services
{
    public class SqlService : ISqlService
    {
        public IConfigurationRoot configutaion;

        public SqlService(IConfigurationRoot configutaion)
        {
            this.configutaion = configutaion;
        }
        public List<Dictionary<string, object>> ExecuteSqlQuery(string dbConnectionString, string sqlQuery)
        {
            using (var con = new SqlConnection(configutaion.GetConnectionString(dbConnectionString)))
            {
                con.Open();

                using (var cmd = new SqlCommand(sqlQuery, con))
                {
                    var reader = cmd.ExecuteReader();
                    var results = new List<Dictionary<string, object>>();

                    while (reader.Read())
                    {
                        results.Add(Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue));
                    }
                    return results;
                }
            }
        }
    }
}