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
        private SqlConnection sc;

        public DBWorker()
        {
            sc = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=D:\Programming\Projects\Project\ServerApplication\ServerApplication\SUperMedDB.mdf;Integrated Security=True");
            //sc = new SqlConnection("Server=.\\SQLEXPRESS;Database=SuperMedDB;Integrated Security=true");
            sc.Open();
        }
        public Dictionary<string, dynamic> SendQuery(string query)
        {
            Dictionary<string, dynamic> response = new Dictionary<string,dynamic>();
            using (SqlCommand command = new SqlCommand(query, sc))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    int j = 0;
                    while (reader.Read())
                    {
                        Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            data.Add(reader.GetName(i), reader.GetValue(i));
                            Console.WriteLine("Row " + j + " Data: {" +reader.GetName(i) + ":" + reader.GetValue(i) + "}");
                        }
                        response.Add(j.ToString(), data);
                        j++;
                        Console.WriteLine();
                    }
                }
            }

            return response;
        }
            
        public void CloseConnection()
        {
            sc.Close();
        }
    }
}
