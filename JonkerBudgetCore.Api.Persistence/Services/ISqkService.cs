using System.Collections.Generic;

namespace SGStatus.WebApi.Persistence.Services
{
    public interface ISqlService
    {
        List<Dictionary<string, object>> ExecuteSqlQuery(string dbConnectionString, string sqlQuery);
    }
}