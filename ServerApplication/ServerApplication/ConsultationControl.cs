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
        public List<string> GetSubsidiaryList(ref Defines.Error error)
        {
            Dictionary<string, dynamic> DBoutput;
            string request = "get_departments";
            DBoutput = DataBaseMessageComposer.SendRequest(request, null);

            List<string> SubsidiaryList = new List<string>();
            foreach (var entry in DBoutput)
                SubsidiaryList.Add(entry.Value);

            if (!SubsidiaryList.Any())
            {
                error.DB_error();
                return null;
            }
            else
            {
                error.Success();
                return SubsidiaryList;
            }
        }
        public Dictionary<string,dynamic> GetDoctorList(Dictionary<string, dynamic> inputData, ref Defines.Error error)
        {
            // checking departments existing
            Defines.Error getDepError = new Defines.Error();
            List<string> SubsidiaryList = this.GetSubsidiaryList(ref getDepError);
            if (getDepError.id != 0)
            {
                error.DB_error();
                return null;
            }

            if(inputData.Count > 1)
            {
                error.BadParameter_error();
                return null;
            }

            if(SubsidiaryList.Contains(inputData["Department"]))
            {
                error.BadParameter_error();
                return null;
            }

            Dictionary<string, dynamic> DBoutput;
            string request = "get_doctors";
            DBoutput = DataBaseMessageComposer.SendRequest(request, null);

            if (!DBoutput.Any())
            {
                error.DB_error();
                return null;
            }
            else
            {
                error.Success();
                return DBoutput;
            }
        }
        public int ReserveTime(Dictionary<string, dynamic> inputData, ref Defines.Error error)
        {
            // compiling request input
            Dictionary<string, dynamic> DBoutput;
            string request = "get_time_status";
            DBoutput = DataBaseMessageComposer.SendRequest(request, inputData);

            if(DBoutput.Count > 0)
            {
                error.Time_oquipied();
                return -1;
            }

            Defines.ReservedTimeItem newTime = new Defines.ReservedTimeItem();
            newTime.Init(inputData);
            reservedTimeList.Add(newTime);
            return 0;
        }       
        public int CreateConsultation(Dictionary<string, dynamic> inputData, ref Defines.Error error)
        {
            Dictionary<string, dynamic> check = CheckRequest(inputData, "check_consultation_existing");
            if (check.Count != 0)
            {
                // cons does not exist
                error.BadParameter_error();
                return -1;
            }

            // compiling request input
            Dictionary<string, dynamic> DBoutput;
            string request = "create_consultation";
            DBoutput = DataBaseMessageComposer.SendRequest(request, inputData);


            if (DBoutput.Count != 1)
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
            Dictionary<string, dynamic> DBoutput;
            string request = "get_consultations";
            DBoutput = DataBaseMessageComposer.SendRequest(request, inputData);

            // parsing request output
            List<Defines.Consultation> ConsList = new List<Defines.Consultation>();
            foreach (var row in DBoutput)
            {
                Defines.Consultation current = new Defines.Consultation();
                //current.Init(row);
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
            Dictionary<string, dynamic> DBoutput;
            string request = "add_note";
            DBoutput = DataBaseMessageComposer.SendRequest(request, inputData);

            if (DBoutput.Count != 1)
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
            Dictionary<string, dynamic> check = CheckRequest(inputData, "check_consultation_existing");
            if (check.Count != 1)
            {
                // cons does not exist
                error.BadParameter_error();
                return -1;
            }

            if (inputData["Username"] != check["Id_Doctor"])
            {
                // you have no permissions
                error.BadParameter_error();
                return -1;
            }

            // compiling request input
            Dictionary<string, dynamic> DBoutput;
            string request = "close_consultation";
            DBoutput = DataBaseMessageComposer.SendRequest(request, inputData);

            // parsing request output
            if (DBoutput.Count != 1)
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
            Dictionary<string, dynamic> check = CheckRequest(inputData, "check_consultation_existing");
            if (check.Count  != 1)
            {
                // cons does not exist
                error.BadParameter_error();
                return -1;
            }

            if (inputData["Username"] != check["Id_Patient"])
            {
                // you have no permissions
                error.BadParameter_error();
                return -1;
            }

            // compiling request input
            Dictionary<string, dynamic> DBoutput;
            string request = "cancel_consultation";
            DBoutput = DataBaseMessageComposer.SendRequest(request, inputData);

            // parsing request output
            if (DBoutput.Count != 1)
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
            Dictionary<string, dynamic> DBoutput;
            string request = "send_message";
            DBoutput = DataBaseMessageComposer.SendRequest(request, inputData);

            // parsing request output
            if (DBoutput.Count == 0)
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
            Dictionary<string, dynamic> DBoutput;
            string request = "get_messages";
            DBoutput = DataBaseMessageComposer.SendRequest(request, inputData);

            // parsing request output
            List<Defines.Message> messagesList = new List<Defines.Message>();
            foreach (var row in DBoutput)
            {
                Defines.Message current = new Defines.Message(
                                                               (string)row.Value["from"],
                                                               (Defines.User)row.Value["to"],
                                                               (string)row.Value["message"]
                                                             );
                messagesList.Add(current);
            }

            if (DBoutput.Count == 0)
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


        private Dictionary<string, dynamic> CheckRequest(Dictionary<string, dynamic> inputData, string request)
        {
            Dictionary<string, dynamic> DBoutput;
            DBoutput = DataBaseMessageComposer.SendRequest(request, inputData);
            return DBoutput;
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
    }
}
