using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defines
{
    //Defines.ConsultationData data = new Defines.ConsultationData();
    public struct ConsultationData
    {
        DateTime date_time;
        String department;
        int patientId;
        int doctorId;

        public ConsultationData(DateTime date_time, String department, int patientId, int doctorId)
        {
            this.date_time = date_time;
            this.department = department;
            this.patientId = patientId;
            this.doctorId = doctorId;
        }
    }
}
