using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database;

namespace Database
{
    public class BaseService : IDisposable
    {
        public DBConnection Connection;
        public string sql { get; set; }
        public BaseService()
        {
            Connection = new DBConnection();
        }
        public void Dispose()
        {
            Connection.CloseConnection();
            sql = "";
        }
    }

}
