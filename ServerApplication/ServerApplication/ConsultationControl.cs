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
        List<Defines.ReservedTimeItem> reservedTimeList;

        public ConsultationControl()
        {
            reservedTimeList = new List<Defines.ReservedTimeItem>();
        }
        public string[] GetDepartmentList(ref Defines.Error error)
        {
            // compiling request input
            DataSet DBoutput = new DataSet();
            string request = "get_departments";
            DBoutput = DataBaseMessageComposer.SendRequest(request, null);

            // parsing request output
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
        private bool Contain(string[] array, dynamic value) 
        {
            int pos = Array.IndexOf(array, value);
            if (pos > -1)
                return true;
            return false;
        }
        public string[] GetDoctorList(Dictionary<string, dynamic> inputData, ref Defines.Error error)
        {
            // checking departments existing
            Defines.Error getDepError = new Defines.Error();
            string[] DepList = this.GetDepartmentList(ref getDepError);
            if (getDepError.id != 0)
            {
                error.DB_error();
                return null;
            }
            //string inputDepartment = 
            bool isDepExist = false;
            foreach (dynamic val in inputData.Values)
            {
                if (Contain(DepList, val))
                {
                    isDepExist = true;
                }
            }

            if (!isDepExist)
            {
                error.BadParameter_error();
                return null;
            }

            // compiling request input
            DataSet DBoutput = new DataSet();
            string request = "get_doctors";
            DBoutput = DataBaseMessageComposer.SendRequest(request, inputData);

            // parsing request output
            List<string> DocList = new List<string>();
            DataTable table = DBoutput.Tables[0];
            foreach (DataRow row in table.Rows)
                DocList.Add((string)row["Doctors"]);

            if (!DocList.Any())
            {
                error.DB_error();
                return null;
            }
            else
            {
                error.Success();
                return DocList.ToArray();
            }
        }
        public int ReserveTime(Dictionary<string, dynamic> inputData, ref Defines.Error error)
        {
            // compiling request input
            DataSet DBoutput = new DataSet();
            string request = "get_time_status";
            DBoutput = DataBaseMessageComposer.SendRequest(request, inputData);

            // parsing request output
            DataRow row = DBoutput.Tables[0].Rows[0];            
            if ( row["status"] != "free" )
            {
                error.BadParameter_error();
                return -1;
            }
            //time is free

            Defines.ReservedTimeItem newTime = new Defines.ReservedTimeItem();
            newTime.Init(inputData);
            reservedTimeList.Add(newTime);
            return 0;
        }

    }
}
