using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
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
        public List<dynamic> GetSubsidiaryList(ref Defines.Error error)
        {
            var response = DataBaseMessageComposer.SendRequest("get_departments", null);

            List<dynamic> SubsidiaryList = new List<dynamic>();
            foreach (var entry in response)
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
        public List<dynamic> GetSpecializationList(ref Defines.Error error)
        {
            var response = DataBaseMessageComposer.SendRequest("get_specializations", null);

            List<dynamic> SpecializationList = new List<dynamic>();
            foreach (var entry in response)
                SpecializationList.Add(entry.Value);

            if (!SpecializationList.Any())
            {
                error.DB_error();
                return null;
            }
            else
            {
                error.Success();
                return SpecializationList;
            }
        }
        public List<dynamic> GetDoctorList(Dictionary<string, dynamic> inputData, ref Defines.Error error)
        {
            Dictionary<string, dynamic> request = new Dictionary<string,dynamic>();
            request.Add("subsidiary", inputData["department"]);
            var response = DataBaseMessageComposer.SendRequest("get_doctors", request);

            List<dynamic> DoctorList = new List<dynamic>();
            foreach (var entry in response)
                DoctorList.Add(entry.Value);

         
            error.Success();
            return DoctorList;
          
        }
        public bool ReserveTime(Dictionary<string, dynamic> inputData, ref Defines.Error error)
        {
            string time = DateTime.Parse(inputData["time"]).ToString("yyyy-MM-dd HH:mm:ss");
            Dictionary<string, dynamic> checkTime = new Dictionary<string, dynamic>
            {
                {"time", time}
            };
            var response = DataBaseMessageComposer.SendRequest("get_time_status", checkTime);

            if(response.Count > 0)
            {
                error.Time_oquipied();
                return false;
            }

            //TODO: Change TImer!!!!
            Defines.ReservedTimeItem newTime = new Defines.ReservedTimeItem();
            newTime.Init(inputData);
            reservedTimeList.Add(newTime);
            return true;
        }       
        public bool CreateConsultation(Dictionary<string, dynamic> inputData, ref Defines.Error error)
        {
            if( (!inputData.ContainsKey("username") || inputData["username"] == null) ||
                (!inputData.ContainsKey("name") || inputData["name"] == null) ||
                (!inputData.ContainsKey("surname") || inputData["surname"] == null) ||
                (!inputData.ContainsKey("patronymic") || inputData["patronymic"] == null) ||
                (!inputData.ContainsKey("specialization") || inputData["specialization"] == null) ||
                (!inputData.ContainsKey("time") || inputData["time"] == null) ||
                (!inputData.ContainsKey("text") || inputData["text"] == null) )
            {
                error.BadParameter_error();
                return false;
            }
            string code = GetRandomString();
            code += GetRandomString();
            string time = DateTime.Parse(inputData["time"]).ToString("yyyy-MM-dd HH:mm:ss");

            //fill data for request
            Dictionary<string, dynamic> request = new Dictionary<string,dynamic>();
            request.Add("username", inputData["username"]);
            request.Add("name", inputData["name"]);
            request.Add("surname", inputData["surname"]);
            request.Add("patronymic", inputData["patronymic"]);
            request.Add("specialization", inputData["specialization"]);
            request.Add("time", time);
            request.Add("code", code);
            request.Add("status", "Created");
            request.Add("text", inputData["text"]);

            var response = DataBaseMessageComposer.SendRequest("create_consultation", request);


            if (response.ContainsValue("CONSULTATION EXISTS"))
            {
                error.Time_oquipied();
                return false;
            }
            else
            {
                if (FreeTime(inputData))
                {
                    error.Server_error();
                    return false;
                }
                error.Success();
                return true;
            }

        }
        public List<dynamic> GetConsultations(Dictionary<string, dynamic> inputData, ref Defines.Error error)
        {
            Dictionary<string, dynamic> request = new Dictionary<string,dynamic>();
            request.Add("status", inputData["status"]);
            request.Add("username", inputData["username"]);

            var response = DataBaseMessageComposer.SendRequest("get_consultations", request);

            // parsing request output
            List<dynamic> ConsList = new List<dynamic>();
            foreach (var row in response)
            {
                if(row.Key == "StartTime")
                {
                    //row.Value = DateTime.Parse(row.Value).ToString("yyyy-MM-dd HH:mm:ss");
                }

                ConsList.Add(row);
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
        public bool AddNote(Dictionary<string, dynamic> inputData, ref Defines.Error error)
        {
            Dictionary<string, dynamic> request = new Dictionary<string,dynamic>();
            request.Add("username", inputData["username"]);
            request.Add("consultationId", inputData["consultationId"]);
            request.Add("text", inputData["text"]);
            var response = DataBaseMessageComposer.SendRequest("add_note", request);

            if (!response.ContainsValue("Note Added"))
            {
                error.DB_error();
                return false;
            }
            else
            {
                error.Success();
                return true;
            }
        }
        public bool CloseConsultation(Dictionary<string, dynamic> inputData, ref Defines.Error error)
        {
            Dictionary<string, dynamic> request = new Dictionary<string,dynamic>();
            request.Add("username", inputData["username"]);
            request.Add("time", inputData["time"]);
            request.Add("consultationId", inputData["consultationId"]);
            request.Add("status", "Closed");
            var response = DataBaseMessageComposer.SendRequest("close_consultation", request);

            // parsing request output
            if (response.ContainsValue("Permission Denied"))
            {
                error.No_Permission();
                return false;
            }
            else
            {
                error.Success();
                return true;
            }
        }
        public bool CancelConsultation(Dictionary<string, dynamic> inputData, ref Defines.Error error)
        {
            Dictionary<string, dynamic> request = new Dictionary<string, dynamic>();
            request.Add("username", inputData["username"]);
            request.Add("consultationId", inputData["consultationId"]);
            request.Add("status", "Closed");
            var response = DataBaseMessageComposer.SendRequest("close_consultation", request);

            // parsing request output
            if (response.ContainsValue("Permission Denied"))
            {
                error.No_Permission();
                return false;
            }
            else
            {
                error.Success();
                return true;
            }
        }
        public bool SendMessage(Dictionary<string, dynamic> inputData, ref Defines.Error error)
        {
            Dictionary<string, dynamic> request = new Dictionary<string, dynamic>();
            request.Add("username", inputData["username"]);
            var response = DataBaseMessageComposer.SendRequest("send_message", request);

            // parsing request output
            if (response.Count == 0)
            {
                error.DB_error();
                return false;
            }
            else
            {
                error.Success();
                return true;
            }
        }
        public List<dynamic> GetMessages(Dictionary<string, dynamic> inputData, ref Defines.Error error)
        {
            Dictionary<string, dynamic> request = new Dictionary<string, dynamic>();
            request.Add("username", inputData["username"]);
            request.Add("type", inputData["type"]);
            var response = DataBaseMessageComposer.SendRequest("get_messages", request);

            List<dynamic> messagesList = new List<dynamic>();
            foreach (var row in response)
            {
                messagesList.Add(row);
            }

            if (response.Count == 0)
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

        private static string GetRandomString()
        {
            string path = Path.GetRandomFileName();
            path = path.Replace(".", ""); // Remove period.
            return path;
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
