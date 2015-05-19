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

        public void RegisterUser(Defines.RegisterUser newUser, ref Defines.Error error)
        {
            Dictionary<string, dynamic> response;
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>()
            {
                {"Name", newUser.user.name},
                {"Surname", newUser.user.surname},
                {"FathersName", newUser.user.fatherName},
                {"DateOfBirth", newUser.dateOfBirth},
                {"Login", newUser.login},
                {"Password", newUser.password},
                {"Approved", false}
            };
            response = DataBaseMessageComposer.SendRequest("register_user", data);
        }

        public void ApproveUser(Defines.UsernameUser user, ref Defines.Error error)
        {

        }

        public void AddRights(Defines.UsernameUser user, ref Defines.Error error)
        {

        }

        public void DeleteUser(Defines.UsernameUser user, ref Defines.Error error)
        {

        }

        public void GetSchedule(Defines.UsernameUser user, ref Defines.Error error)
        {

        }

        public void ChangeSchedule(Defines.Schedule schedule, Defines.UsernameUser user, ref Defines.Error error)
        {

        }

        public void VacationPlanning(Defines.Vacation vacationData, ref Defines.Error error)
        {

        }

        public void ApprovePlan(Defines.UsernameUser user, ref Defines.Error error)
        {

        }
    }
}
