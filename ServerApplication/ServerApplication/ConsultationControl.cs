using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;

namespace ServerApplication
{
    class ConsultationControl
    {


        public ConsultationControl()
        {
        }
        //это стаб, пустышка, вместо null он должен возвращать массив данных, или пустой массив.
        public string[] GetDepartmentList(ref Defines.Error error)
        {
            DataSet DBoutput = new DataSet();
            string request = "GetDepartmentList";
            DBoutput = DataBaseMessageComposer.SendRequest(request);
            
            List<string> DepList = new List<string>();
            DataTable table = DBoutput.Tables[0];
            foreach (DataRow row in table.Rows)
                DepList.Add((string)row["Departments"]);

            if (!DepList.Any())
            {
                error.DB_error();
                return null;
            }
            else
            {
                error.Success();
                return DepList.ToArray();
            }
        }

    }
}
