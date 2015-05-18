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
            if (row["status"] != "free")
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
        private bool FreeTime(Dictionary<string, dynamic> inputData)
        {
            bool Success = inputData.ContainsKey("time");
            if (!Success)
                return false;

            dynamic timeForDelete = new DateTime();
            Success = inputData.TryGetValue("time", timeForDelete);

            if (!Success)
                return false;

            foreach (Defines.ReservedTimeItem val in reservedTimeList)
            {
                if (val.time.Equals(timeForDelete))
                {
                    int indexForDelete = reservedTimeList.IndexOf(val);
                    reservedTimeList.Remove(val);
                }
            }

            return true;
        }
        private bool CheckRequest(Dictionary<string, dynamic> inputData, string request)
        {
            DataSet DBoutput = new DataSet();
            DBoutput = DataBaseMessageComposer.SendRequest(request, inputData);
            if (DBoutput.Tables[0].Rows.Count != 0)
            {
                return true;
            }
            return false;
        }
        public int CreateConsultation(Dictionary<string, dynamic> inputData, ref Defines.Error error)
        {
            if (CheckRequest(inputData, "check_consultation_existing"))
            {
                // cons already exist
                error.BadParameter_error();
                return -1;
            }

            if (!CheckRequest(inputData, "check_permissions_create"))
            {
                // you have no permissions
                error.BadParameter_error();
                return -1;
            }

            // compiling request input
            DataSet DBoutput = new DataSet();
            string request = "create_consultation";
            DBoutput = DataBaseMessageComposer.SendRequest(request, inputData);

            // parsing request output
            if (DBoutput.Tables[0].Rows.Count == 0)
            {
                error.DB_error();
                return -1;
            }
            else
            {
                if (FreeTime(inputData))
                {
                    error.Server_error();
                    return -1;
                }
                error.Success();
                return 0;
            }

        }
        public List<Defines.Consultation> GetConsultations(Dictionary<string, dynamic> inputData, ref Defines.Error error)
        {
            // compiling request input
            DataSet DBoutput = new DataSet();
            string request = "get_consultations";
            DBoutput = DataBaseMessageComposer.SendRequest(request, inputData);

            // parsing request output
            List<Defines.Consultation> ConsList = new List<Defines.Consultation>();
            DataTable table = DBoutput.Tables[0];
            foreach (DataRow row in table.Rows)
            {
                Defines.Consultation current = new Defines.Consultation();
                current.Init(row);
                ConsList.Add(current);
            }

            if (!ConsList.Any())
            {
                error.DB_error();
                return null;
            }
            else
            {
                error.Success();
                return ConsList;
            }
        }
        public int AddNote(Dictionary<string, dynamic> inputData, ref Defines.Error error)
        {
            if (!CheckConsultationExisting(inputData))
            {
                error.BadParameter_error();
                return -1;
            }

            // compiling request input
            DataSet DBoutput = new DataSet();
            string request = "add_note";
            DBoutput = DataBaseMessageComposer.SendRequest(request, inputData);

            // parsing request output
            if (DBoutput.Tables[0].Rows.Count == 0)
            {
                error.DB_error();
                return -1;
            }
            else
            {
                error.Success();
                return 0;
            }
        }
        public int CloseConsultation(Dictionary<string, dynamic> inputData, ref Defines.Error error)
        {
            if (!CheckRequest(inputData, "check_consultation_existing"))
            {
                // cons does not exist
                error.BadParameter_error();
                return -1;
            }

            if (!CheckRequest(inputData, "check_permissions_close"))
            {
                // you have no permissions
                error.BadParameter_error();
                return -1;
            }

            // compiling request input
            DataSet DBoutput = new DataSet();
            string request = "close_consultation";
            DBoutput = DataBaseMessageComposer.SendRequest(request, inputData);

            // parsing request output
            if (DBoutput.Tables[0].Rows.Count == 0)
            {
                error.DB_error();
                return -1;
            }
            else
            {
                error.Success();
                return 0;
            }
        }
        public int CancelConsultation(Dictionary<string, dynamic> inputData, ref Defines.Error error)
        {
            if (!CheckRequest(inputData, "check_consultation_existing"))
            {
                // cons does not exist
                error.BadParameter_error();
                return -1;
            }

            if (!CheckRequest(inputData, "check_permissions_cancel"))
            {
                // you have no permissions
                error.BadParameter_error();
                return -1;
            }

            // compiling request input
            DataSet DBoutput = new DataSet();
            string request = "cancel_consultation";
            DBoutput = DataBaseMessageComposer.SendRequest(request, inputData);

            // parsing request output
            if (DBoutput.Tables[0].Rows.Count == 0)
            {
                error.DB_error();
                return -1;
            }
            else
            {
                error.Success();
                return 0;
            }
        }
        public int SendMessage(Dictionary<string, dynamic> inputData, ref Defines.Error error)
        {
            DataSet DBoutput = new DataSet();
            string request = "send_message";
            DBoutput = DataBaseMessageComposer.SendRequest(request, inputData);

            // parsing request output
            if (DBoutput.Tables[0].Rows.Count == 0)
            {
                error.DB_error();
                return -1;
            }
            else
            {
                error.Success();
                return 0;
            }
        }
        public List<Defines.Message> GetMessages(Dictionary<string, dynamic> inputData, ref Defines.Error error)
        {
            DataSet DBoutput = new DataSet();
            string request = "get_messages";
            DBoutput = DataBaseMessageComposer.SendRequest(request, inputData);

            // parsing request output
            List<Defines.Message> messagesList = new List<Defines.Message>();
            DataTable table = DBoutput.Tables[0];
            foreach (DataRow row in table.Rows)
            {
                Defines.Message current = new Defines.Message(
                                                               (string)row["from"],
                                                               (Defines.User)row["to"],
                                                               (string)row["message"]
                                                             );
                messagesList.Add(current);
            }

            if (DBoutput.Tables[0].Rows.Count == 0)
            {
                error.DB_error();
                return null;
            }
            else
            {
                error.Success();
                return messagesList;
            }
        }

    }
}
