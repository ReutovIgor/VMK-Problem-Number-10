using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;

namespace ServerApplication
{
    class UserControl
    {
        public UserControl(){}

        public List<dynamic> GetPendings(ref Defines.Error error)
        {
            var response = DataBaseMessageComposer.SendRequest("get_pendings", null);
            List<dynamic> list = new List<dynamic>();
            foreach(var pair in response)
            {
                list.Add(pair.Value);
            }
            return list;
        }

        public bool RegisterUser(Dictionary<string, dynamic> inputData, ref Defines.Error error)
        {
            Dictionary<string, dynamic> Data = new Dictionary<string, dynamic>();
            Data.Add("name", inputData["name"]);
            Data.Add("surname", inputData["surname"]);
            Data.Add("patronymic", inputData["patronymic"]);
            Data.Add("username", inputData["username"]);
            Data.Add("password", inputData["password"]);
            Data.Add("date", DateTime.Parse(inputData["date"]).ToString("yyyy-MM-dd"));

            var response = DataBaseMessageComposer.SendRequest("register_user", Data);

            if(response.Count != 1)
            {
                error.DB_error();
                return false;
            }
            return true;
        }
        
        public Dictionary<string, dynamic> Login(Dictionary<string, dynamic> inputData, ref Defines.Error error)
        {
            var response = DataBaseMessageComposer.SendRequest("login", inputData);
            if ((response.Count != 1) && (response["0"].ContainsValue("Username or Password are incorrrect")))
            {
                error.Incorrect_Username_Password();
                return null;
            }
            return response["0"];
        }

        public bool ApproveUser(Dictionary<string, dynamic> inputData, ref Defines.Error error)
        {
            var response = DataBaseMessageComposer.SendRequest("approve_user", inputData);

            if (response.Count != 1)
            {
                error.DB_error();
                return false;
            }
            return true;
        }

        public List<dynamic> GetUsers(ref Defines.Error error)
        {
            var response = DataBaseMessageComposer.SendRequest("get_users", null);
            List<dynamic> list = new List<dynamic>();
            foreach (var pair in response)
            {
                list.Add(pair.Value);
            }
            return list;
        }

        public bool AddRights(Dictionary<string, dynamic> inputData, ref Defines.Error error)
        {
            string request;
            Dictionary<string, dynamic> Data = new Dictionary<string, dynamic>();
            Data.Add("name", inputData["name"]);
            Data.Add("surname", inputData["surname"]);
            Data.Add("patronymic", inputData["patronymic"]);

            string str = inputData["right"];
            switch (str)
            {
                case "Manager":
                    request = "add_rights_manager";
                    break;
                case "Doctor":
                    request = "add_rights_doctor";
                    Data.Add("username", inputData["username"]);
                    Data.Add("subsidiary", inputData["subsidiary"]);
                    Data.Add("specialization", inputData["specialization"]);
                    Data.Add("monday", inputData["timetable"]["Monday"]);
                    Data.Add("tuesday", inputData["timetable"]["Tuesday"]);
                    Data.Add("wednesday", inputData["timetable"]["Wednesday"]);
                    Data.Add("thursday", inputData["timetable"]["Thursday"]);
                    Data.Add("friday", inputData["timetable"]["Friday"]);
                    Data.Add("saturday", inputData["timetable"]["Saturday"]);
                    Data.Add("sunday", inputData["timetable"]["Sunday"]);
                    break;
                default:
                    error.BadParameter_error();
                    return false;
            }
            var response = DataBaseMessageComposer.SendRequest(request, Data);

            if (response.Count != 1)
            {
                error.DB_error();
                return false;
            }
            return true;
        }

        public List<dynamic> GetSchedule(Dictionary<string, dynamic> inputData, ref Defines.Error error)
        {
            Dictionary<string, dynamic> Data = new Dictionary<string, dynamic>();
            Data.Add("name", inputData["name"]);
            Data.Add("surname", inputData["surname"]);
            Data.Add("patronymic", inputData["patronymic"]);
            Data.Add("username", inputData["username"]);

            var response = DataBaseMessageComposer.SendRequest("get_schedule", Data);

            if (response.Count != 1)
            {
                error.DB_error();
                return null;
            }

            List<dynamic> schedule = new List<dynamic>();
            schedule.Add(response["0"]);
            
            return schedule;
        }

        public bool ChangeSchedule(Dictionary<string, dynamic> inputData, ref Defines.Error error)
        {
            Dictionary<string, dynamic> Data = new Dictionary<string, dynamic>();
            Data.Add("name", inputData["name"]);
            Data.Add("surname", inputData["surname"]);
            Data.Add("patronymic", inputData["patronymic"]);

            string days = inputData["days"];
            string workTime = inputData["time"];

            string[] daysArr = days.Split(';');
            string[] workTimeArr = workTime.Split(';');

            if(daysArr.Count() != workTime.Count())
            {
                error.BadParameter_error();
                return false;
            }
            string update = "";
            for (int i = 0; i < daysArr.Count(); i++)
            {
                update +=daysArr[i] + "=" + workTimeArr[i] + " ";
            }

            Data.Add("update", update);

            var response = DataBaseMessageComposer.SendRequest("change_schedule", Data);

            if ((response.Count != 1) && (!response["0"].ContainsValue("TimeTable updated")))
            {
                error.DB_error();
                return false;
            }
            return true;
        }

        public bool VacationPlanning(Dictionary<string, dynamic> inputData, ref Defines.Error error)
        {
            Dictionary<string, dynamic> Data = new Dictionary<string, dynamic>();
            Data.Add("username", inputData["username"]);
            Data.Add("vacation", inputData["plan"]);
            Data.Add("time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            Data.Add("text", inputData["Please approve my vacation plan"]);

            var response = DataBaseMessageComposer.SendRequest("vacation_planning", Data);

            if ((response.Count != 1) && (!response["0"].ContainsValue("Vacation plan is added")))
            {
                error.DB_error();
                return false;
            }

            return true;
        }

        public bool Approve_RejectPlan(Dictionary<string, dynamic> inputData, ref Defines.Error error)
        {
            string request = "";
            Dictionary<string, dynamic> Data = new Dictionary<string, dynamic>();
            Data.Add("name", inputData["name"]);
            Data.Add("surname", inputData["surname"]);
            Data.Add("patronymic", inputData["patronymic"]);
            Data.Add("usernmae", inputData["username"]);
            Data.Add("time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            

            if(inputData["decision"])
            {
                request = "approve_plan";
                Data.Add("text", "Vacation plan approved");
            }
            else
            {
                request = "reject_plan";
                Data.Add("text", "Vacation plan rejected, see comments below: " + inputData["comments"]);
            }

            var response = DataBaseMessageComposer.SendRequest(request, Data);

            if ((response.Count != 1) && ((!response["0"].ContainsValue("Approval message was added")) || 
                                          (!response["0"].ContainsValue("Reject message was added"))))
            {
                error.DB_error();
                return false;
            }

            return true;
        }
    }
}
