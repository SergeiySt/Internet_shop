using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Data.SqlClient;
using System.Configuration;
using Dapper;
using System.Windows.Forms;
using System.Net.PeerToPeer;

namespace InternetShop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string connectionString;
        public MainWindow()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["ConnectionDB"].ConnectionString;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WUserRegister wUserRegister = new WUserRegister();
            wUserRegister.Show();
        }

        private void buttonSingIn_Click(object sender, RoutedEventArgs e)
        {
            string login = textBoxLogin.Text;
            string password = new System.Net.NetworkCredential(string.Empty, passwordBoxPassword.SecurePassword).Password;
            
            string surName = "";
            string Name = "";
            string pobatkovi = "";



            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var clients = db.Query<InfoDB.Client>("SELECT * from Client where CLogin = @CLogin and CPassword = @CPassword",
                    new { CLogin = login, CPassword = password }).ToList();

                if (clients.Count == 1)
                {
                    surName = clients[0].CSurname;
                    Name = clients[0].CName;
                    pobatkovi = clients[0].CPobatkovi;

                    WUser wUser = new WUser(surName, Name, pobatkovi);
                    wUser.Show();
                }
                else
                {
                    var personnel = db.Query<InfoDB.Personnel>("select * from Personnel where PLogin = @PLogin and PPassword = @PPassword",
                        new { PLogin = login, PPassword = password }).ToList();

                    if (personnel.Count == 1)
                    {
                        surName = personnel[0].PSurname;
                        Name = personnel[0].PName;
                        pobatkovi = personnel[0].PPobatkovi;

                        var admin = personnel.First();

                        if (admin.PRole == "admin")
                        {
                            WAdmin wAdmin = new WAdmin(surName, Name, pobatkovi);
                            wAdmin.Show();
                        }
                        else
                        {
                            WUser wUser = new WUser(surName, Name, pobatkovi);
                            wUser.Show();
                        }
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Невірний логін або пароль", "Помилка авторизації", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
            }
            this.Close();
        }
    }
}
