using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ServerApplication
{
    static class DataBaseMessageComposer
    {
        private static DBWorker dbWorker;

        static DataBaseMessageComposer()
        {
            dbWorker = new DBWorker();
        }

        public static DataSet SendRequest(string query)
        {
            return dbWorker.SendQuery(query);
        }
    }
}
