using Council.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Data
{
    public class DataBase
    {
        protected readonly DbContext dbContext;
        public DataBase()
        {
            dbContext = ContextPerRequest.MainContext;//default context
        }
        public DataBase(string contextName)
        {
            switch (contextName)
            {
                case "MainContext":
                    dbContext = ContextPerRequest.MainContext;
                    break;
            }
        }

        public string ServerDateTime()
        {
            //var sqlDateTime = dbContext.Database.ExecuteSqlCommand("select getdate()");
            //var sqlDateTime = dbContext.Database.SqlQuery<string>("select getdate()");
            //var result = sqlDateTime.ToString();

            using (var cn = new SqlConnection(Council.Core.Values.Values.MainConnectionString))
            {
                DateTime dt = new DateTime();
                cn.Open();
                using (var cmd = new SqlCommand("select getdate()", cn))
                {
                    var date = cmd.ExecuteScalar();
                    return date.ToString();
                }
            }
        }

        public IList<T> Select<T>(string viewName, string condition = null)
        {
            string query = "select * from " + viewName + (condition == null ? "" : condition);
            return dbContext.Database.SqlQuery<T>(query).ToList();
        }

        public T Get<T>(string viewName, string condition = null)
        {
            string query = "select * from " + viewName + " " + (condition == null ? "" : condition);
            return dbContext.Database.SqlQuery<T>(query).First();
        }


        public IList<T> SelectFromStoreProcedure<T>(string query)
        {
            return dbContext.Database.SqlQuery<T>(query).ToList();
        }
    }
}
