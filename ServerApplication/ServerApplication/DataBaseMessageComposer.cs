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
        private static Dictionary<string, string> queryTemplates;

        static DataBaseMessageComposer()
        {
            dbWorker = new DBWorker();
            queryTemplates = new Dictionary<string, string>()
            {
                {"get_departments", "SELECT * FROM Felial"},
                {"get_doctors", "SELECT * FROM "}
            };
        }

        public static DataSet SendRequest(string query, Dictionary<string, dynamic> data)
        {
            return dbWorker.SendQuery("");
        }
    }
}
