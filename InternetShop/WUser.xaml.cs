using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using System.Data.SqlClient;
using System.Configuration;
using Dapper;
using static InternetShop.InfoDB;

namespace InternetShop
{
    /// <summary>
    /// Interaction logic for WUser.xaml
    /// </summary>
    public partial class WUser : Window
    {
        private string connectionString;
        public WUser()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["ConnectionDB"].ConnectionString;
        }

        public void SelectedGoods()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var goods = connection.Query<Goods>("SELECT * FROM Goods");
                var selectedGood = goods.FirstOrDefault();

                textBlockGoods.Text = $"{selectedGood.GName} {selectedGood.GBrand} {selectedGood.GPrice} {selectedGood.GDescription}";
            }

            
        }
    }
}
