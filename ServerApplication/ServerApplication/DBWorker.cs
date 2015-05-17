using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Data.SqlClient;

namespace ServerApplication
{
    class DBWorker
    {
        private SqlCeConnection sc;

        public DBWorker()
        {
            string connectionString = "Data Source=DBSuperMed.sdf";
            sc = new SqlCeConnection(connectionString);
            sc.Open();
        }

        public DataSet SendQuery(string query) {
            SqlCeDataAdapter dbAdapter = new SqlCeDataAdapter(query, sc);
            DataSet dtSet = new DataSet();
            dbAdapter.Fill(dtSet);
            return dtSet;
        }
            
        public void CloseConnection()
        {
            sc.Close();
        }
    }
}
