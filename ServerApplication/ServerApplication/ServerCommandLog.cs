using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerApplication
{
    public partial class ServerCommandLog : Form
    {
        public ServerCommandLog()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //string query = "select table_name from information_schema.tables where TABLE_TYPE <> 'VIEW'";
            //string query = "INSERT INTO Man (Name, Surname, Patronymic, DateOfBirth, Username, Password, Approved) Values ('asd', 'sdf', 'sss', convert(date,'18-06-12',5), 's1', 's2', 1)";
            string query = "INSERT INTO Man (Name, Surname, Patronymic, DateOfBirth, Username, Password, Approved) Values ('asd1', 'sdf', 'sss', convert(date,'19-09-1989',105), 's1', 's2', 1)";
            Dictionary<string, dynamic> data = DataBaseMessageComposer.SendRequest(query, null);
            
        }
    }
}
