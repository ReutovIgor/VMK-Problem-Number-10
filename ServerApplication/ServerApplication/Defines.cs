using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defines
{
    #region Predefined ConsultationControl Structures
    public struct User
    {
        public String name;
        public String surname;
    }
    
    public struct CreateConsultation
    {
        public DateTime date_time;
        public String department;
        public String username;
        public User doctor;
        public String message;
    }

    public struct ReserveTime
    {
        public DateTime time;
        public String username;
    }

    public struct AddNote
    {
        public String username;
        public int ConsultationId;
    }

    public struct GetConsultations
    {
        public String username;
        public int status;
    }

    public struct FinishConsultation
    {
        public int consultationId;
        public String username;
    }

    public struct SendMessage
    {
        public String from;
        public User to;
        public String message;
    }
    #endregion

    #region Predefined UserControl Structures

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
