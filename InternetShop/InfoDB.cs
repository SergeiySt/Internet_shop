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
            public int id { get; set; }
            public string CSurname { get; set; }
            public string CName { get; set; }
            public string CPobatkovi { get; set; }
            public string CEmail { get; set; }
            public int LPhone { get; set; }
            public string CLogin { get; set; }
            public string CPassword { get; set; }
        }

        public class Personnel
        {
            public int id { get; set; }
            public string PSurname { get; set; }
            public string PName { get; set; }
            public string PPobatkovi { get; set; }
            public string PLogin { get; set; }
            public string PPassword { get; set; }
            public string PRole { get; set; }
        }

    }
}
