using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defines
{
    #region Predefined ConsultationControl Structures
    public struct CreateConsultation
    {
        public DateTime date_time;
        public String department;
        public User patient;
        public User doctor;
        public String message;
    }

    public struct ReservedTimeItem
    {
        DateTime time;
        string username;

        public void Init(Dictionary<string, dynamic> input)
        {
            DateTime inputTime = input.Values.First();
            this.time = new DateTime(   inputTime.Year,
                                        inputTime.Month,
                                        inputTime.Day,
                                        inputTime.Hour,
                                        inputTime.Minute,
                                        inputTime.Second);
            this.username = input.Keys.First();
        }
    }

    public struct EditConsultation
    {
        public String username;
        public int ConsultationId;
    }

    public struct GetConsultations
    {
        public String username;
        public int status;
    }

    
    #endregion

    #region Predefined UserControl Structures
    public struct RegisterUser
    {
        public User user;
        public String dateOfBirth;
        public String login;
        public String password;
    }

    public struct UsernameUser
    {
        public String username;
        public User user;
    }

    public struct Schedule
    {
        public DaySchedule Monday;
        public DaySchedule Tuesday;
        public DaySchedule Wednesday;
        public DaySchedule Thursday;
        public DaySchedule Friday;
        public DaySchedule Saturday;
        public DaySchedule Sunday;
    }

    public struct DaySchedule
    {
        public String[] from;
        public String[] to;

        public DaySchedule(String[] From, String[] To)
        {
            this.from = From;
            this.to = To;
        }
    }

    public struct Vacation
    {
        public String username;
        public String[] from;
        public String[] to;
    }
    #endregion

    #region Predefined Common Structures
    public struct User
    {
        public String name;
        public String surname;
        public String fatherName;
    }
    
    public struct SendMessage
    {
        public String from;
        public User to;
        public String message;
    }
    #endregion

    #region Errors
    public struct Error
    {
        public int id;
        public String text;

        public void Success()
        {
            this.id = 0;
            this.text = "Success";
        }
        public void DB_error()
        {
            this.id = 100;
            this.text = "DataBase Error";
        }

        public void BadParameter_error()
        {
            this.id = 101;
            this.text = "Bad Parameters passed";
        }

        public void Server_error()
        {
            this.id = 102;
            this.text = "Server Error";
        }
    }
    #endregion

}
