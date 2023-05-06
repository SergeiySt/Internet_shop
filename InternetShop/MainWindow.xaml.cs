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

        private void buttonConnectDB_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionDB"].ConnectionString;
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                try
                {
                    db.Open();
                    MessageBox.Show("Connection successful.", "Примітка", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Connection failed: " + ex.Message, "Примітка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
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

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var clients = db.Query<InfoDB.Client>("SELECT * from Client where CLogin = @CLogin and CPassword = @CPassword",
                    new { CLogin = login, CPassword = password }).ToList();

                if (clients.Count == 1)
                {
                   // var client = clients.First();
                    WUser wUser = new WUser();
                    wUser.Show();
                }
                else
                {
                    var personnel = db.Query<InfoDB.Personnel>("select * from Personnel where PLogin = @PLogin and PPassword = @PPassword",
                        new { PLogin = login, PPassword = password }).ToList();

                    if (personnel.Count == 1)
                    {
                        var admin = personnel.First();
                        // Пользователь найден в таблице Personnel - он администратор, если его роль равна "admin"
                        if (admin.PRole == "admin")
                        {
                            WAdmin wAdmin = new WAdmin();
                            wAdmin.Show();
                        }
                        else
                        {
                            // Пользователь найден в таблице Personnel, но не является администратором
                            WUser wUser = new WUser();
                            wUser.Show();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Невірний логін або пароль", "Помилка авторизації");
                    }
                }
            }
        }

        //private bool IsUserAdmin(InfoDB.Client user)
        //{
        //    using (IDbConnection db = new SqlConnection(connectionString))
        //    {
        //        var admins = db.Query<InfoDB.Personnel>("select * from Personnel where PLogin = @PLogin",
        //             new { PLogin = user.CLogin }).ToList();

        //        return admins.Count == 1 && admins.First().PRole == "admin";
        //    }
        //}
    }
}
