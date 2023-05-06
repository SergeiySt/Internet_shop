using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace InternetShop
{
    public class InfoDB
    {
        public class Client
        {
            //public Client() { }
            public int id { get; set; }
            public string CSurname { get; set; }
            public string CName { get; set; }
            public string CPobatkovi { get; set; }
            public string CEmail { get; set; }
            public int LPhone { get; set; }
            public string CLogin { get; set; }
            public string CPassword { get; set; }
        }
    }
}
