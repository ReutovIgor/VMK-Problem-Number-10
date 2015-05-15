using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApplication
{ 
    class ConsultationControl
    {
        private List<Department> DepList;
        private List<Doctor> DocList;
        private DataBaseMessageComposer DBComposer;
        public TimeMomentsList ReservedTime;

        public ConsultationControl()
        {
            DepList = new List<Department>();
            DocList = new List<Doctor>();
            DBComposer = new DataBaseMessageComposer();
            ReservedTime = new TimeMomentsList();
        }

        public List<Department> GetDepartmentList(Response InResponse)
        {
            Error curr =  DBComposer.SendRequest(ref InResponse);
            if (curr == Error.Success)
                DepList = InResponse.DepList;
            else DepList.Clear();

            return DepList;
        }

        public List<Doctor> GetDoctorList(Response InResponse)
        {
            Error curr = DBComposer.SendRequest(ref InResponse);
            if (curr == Error.Success)
                DocList = InResponse.DocList;
            else DocList.Clear();

            return DocList;
        }

        private Error ReserveTime(TimeMoment Moment)
        {
            if (InList(Moment))
                return Error.Fail;
            else
            {
                ReservedTime.TimeList.Add(Moment);
                return Error.Success;
            }
        }

        private bool InList(TimeMoment InMoment)
        {
            return ReservedTime.Exists(x => x.data == InMoment.data && x.start == InMoment.start && x.finish == InMoment.finish);
        }

        public Error CreateConsultation(Response InResponse)
        {
            Error IsReserved = ReserveTime(InResponse.timeMoment);
            if (IsReserved == Error.Success)
                return DBComposer.SendRequest(ref InResponse);
            else
                return Error.Fail;
        }

        public Error AddNote(ref Response InResponse)
        {
            Response PermissionsRequest = new Response();
            PermissionsRequest.ID = 4;//and other initialization
            Error haveAccess = DBComposer.SendRequest(PermissionsRequest);
            if (haveAccess == Error.Success)
                return DBComposer.SendRequest(ref InResponse);
            return Error.Fail;
        }
        public Error CloseConsultation(ref Response InResponse)
        {
            return DBComposer.SendRequest(ref InResponse); 
        }

        public Error SubmitConsultation(ref Response InResponse)
        {
            Response PermissionsRequest = new Response();
            PermissionsRequest.ID = 7;
            PermissionsRequest.ConsID = InResponse.ConsID;
            PermissionsRequest.PatID = InResponse.PatID;
            Error haveAccess = DBComposer.SendRequest(PermissionsRequest);
            if (haveAccess == Error.Success)
                return CloseConsultation(ref InResponse);
            return Error.Fail;
        }

        public Error CancelConsultation(ref Response InResponse)
        {
            Response PermissionsRequest = new Response();
            PermissionsRequest.ID = 6;
            PermissionsRequest.ConsID = InResponse.ConsID;
            PermissionsRequest.PatID = InResponse.PatID;
            Error haveAccess = DBComposer.SendRequest(PermissionsRequest);
            if (haveAccess == Error.Success)
                return CloseConsultation(ref InResponse);
            return Error.Fail;
        }
    }


    public struct Response
    {
        public int ID; // ResponseID
                // 1 - GetDepartmentList
                // 2 - GetDoctorList
                // 3 - ReserveTime
                // 4 - CreateConsultaion
                // 5 - AddNote
                // 6 - CancelConsultation
                // 7 - SubmitConsultation
        public List<Department> DepList;
        public int DepID;
        public List<Doctor> DocList;
        public TimeMoment timeMoment;
        public int DocID;
        public int PatID;
        public int ConsID;
        public string Note;
        /*
        public Response(int ID, List<Department> DepList, List<Doctor> DocList)
        {
            this.ID = ID;
            this.DepList = DepList;
            this.DocList = DocList;
        }*/
    }
    public struct TimeMomentsList
    {
        public List<TimeMoment> TimeList; 
    }
    public struct TimeMoment
    {
        public CalendarData data;
        public clock start;
        public clock finish;
    }
    public struct CalendarData
    {
        public int day;
        public int month;
        public int year;
    }
    public struct clock
    {
        public int hour;
        public int minute;
    }

    public enum Error
    { 
        Success,
        Fail,
    }

    public struct Department
    {
        int ID;
        string Address;

        public Department(int ID, string Address)
        {
            this.ID = ID;
            this.Address = Address;
        }
    }

    public struct Doctor
    {
        int ID;
        string Name;
        int DepartmentID;

        public Doctor(int ID, string Name, int DepartmentID)
        {
            this.ID = ID;
            this.Name = Name;
            this.DepartmentID = DepartmentID;
        }
    }
}
