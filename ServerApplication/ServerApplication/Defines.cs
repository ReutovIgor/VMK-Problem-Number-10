using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Defines
{
    public struct ReservedTimeItem
    {
        public DateTime time;
        public string username;

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
        public void No_Permission()
        {
            this.id = 103;
            this.text = "No Permission!";
        }
        public void Method_Not_Supported()
        {
            this.id = 104;
            this.text = "Method not supported";
        }
        public void Time_oquipied()
        {
            this.id = 200;
            this.text = "This Time is Taken";
        }
        public void Incorrect_Username_Password()
        {
            this.id = 300;
            this.text = "Username or Password are incorrect";

        }
    }
    #endregion

}
