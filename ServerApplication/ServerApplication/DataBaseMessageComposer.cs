using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ServerApplication
{
    static class DataBaseMessageComposer
    {
        private static DBWorker dbWorker;
        private static Dictionary<string, string> queryTemplates;

        static DataBaseMessageComposer()
        {
            dbWorker = new DBWorker();
            queryTemplates = new Dictionary<string, string>()
            {
                //Message Control Authorization check query
                {"check_authorization", "SELECT Id_<table> FROM <table> WHERE Id_Man IN (SELECT Id_Man FROM Man WHERE Username IN (<username>)"},
                //Consultation Control queries
                {"get_departments", "SELECT * FROM Subsidiary"},
                {"get_doctors", "SELECT * FROM Doctor WHERE Id_Subsidiary IN ('<subsidiary>')"},
                {"get_time_status", "SELECT * FROM Consultation WHERE ConsultationTime IN ('<time>')"},
                {"create_consultation", "INSERT INTO Consultation (Id_Patient, Id_Doctor, Begin, Code, Status, PatientText) VALUES ('<username>', '<doctorId>', '<begin>', '<code>', '<status>', '<text>')"},
                {"get_consultations", "SELECT * FROM Consultation WHERE Status IN ('<status>')"},
                {"add_note", "UPDATE Consultation SET DoctorText='<text>' WHERE Id_Consultation='<consultationId>'"},
                {"close_consultation", "UPDATE Consultation SET End='<end>',Status='Closed'"},
                {"cancel_consultation", "UPDATE Consultation SET Status='Canceled'"},
                {"send_message", "INSERT INTO Dialog (Id_OnlineConsultation, Author, Message, Time) VALUES ('<onlineConsultationId>', '<username>', '<text>', '<time>')"},
                {"get_messages", "SELECT * FROM Dialog WHERE Id_OnlineConsultation IN (SELECT Id_OnlineConsultation FROM OnlineConsultation WHERE Id_Doctor='<doctorId>' OR Id_Patient='<patientId>')"},
                {"check_consultation_existing", "SELECT * FROM Consultation WHERE Id_Consultation IN ('<consultationId>')"},
                {"", ""}
                //User Control queries
            };
        }

        public static Dictionary<string, dynamic> SendRequest(string query, Dictionary<string, dynamic> data)
        {
            return dbWorker.SendQuery(query);
        }
    }
}
