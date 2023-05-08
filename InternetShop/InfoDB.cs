﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using Dapper.Contrib.Extensions;


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


        public class Goods
        {
            public int id_goods { get; set; }
            public string GName { get; set; }
            public string GType { get; set; }
            public string GBrand { get; set; }
            public decimal GPrice { get; set; }
            public string GDescription { get; set; }
            public int GCount { get; set; }
            public byte[] GPicture { get; set; }

            [OneToMany("id_goods", "id_goods")]
            public ICollection<GoodsOrder> GoodsOrders { get; set; }
        }

        public class GoodsOrder
        {
            public int id_goodsOrder { get; set; }
            public int id_goods { get; set; }
            public int OCount { get; set; }
            public string OAdress { get; set; }
            public bool OStatus { get; set; }

            [ManyToOne("id_goods")]
            public Goods Goods { get; set; }
        }
    }
}
