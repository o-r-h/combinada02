using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using static Veritrade2018.Helpers.Variables;

namespace Veritrade2018.Helpers
{
    public class ConexProvider
    {
        DbConnection conn;
        string connectionString;
        DbProviderFactory factory;

        
        // Constructor that retrieves the connectionString from the config file
        public ConexProvider(ConfigManager cm = ConfigManager.SYSTEM)
        {
#if DEBUG
            var val = string.Concat("Local", cm.GetDn());
#else
            var val = string.Concat("Remote", cm.GetDn());
#endif
            this.connectionString = ConfigurationManager.ConnectionStrings[val].ConnectionString;
            factory = DbProviderFactories.GetFactory(ConfigurationManager.ConnectionStrings[val].ProviderName);
        }


        // Constructor that accepts the connectionString and Database ProviderName i.e SQL or Oracle
        public ConexProvider(string connectionString, string connectionProviderName)
        {
            this.connectionString = connectionString;
            factory = DbProviderFactories.GetFactory(connectionProviderName);
        }

        //// Only inherited classes can call this.
        public IDbConnection Open
        {
            get
            {
                conn = factory.CreateConnection();
                conn.ConnectionString = this.connectionString;
                conn.Open();

                return conn;
            }
        }
    }
}