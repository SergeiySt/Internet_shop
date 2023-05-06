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
using System.Windows.Shapes;

using System.Data.SqlClient;
using System.Configuration;
using Dapper;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Text.RegularExpressions;
using static InternetShop.InfoDB;

namespace InternetShop
{
    /// <summary>
    /// Interaction logic for WUserRegister.xaml
    /// </summary>
    public partial class WUserRegister : Window
    {
        private string connectionString; 
        public WUserRegister()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["ConnectionDB"].ConnectionString;
        }
        public void AddUser(InfoDB.Client client)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string query = "Insert into Client (CSurname, CName, CPobatkovi, CEmail, LPhone, CLogin, CPassword) " +
                    "values (@CSurname, @CName, @CPobatkovi, @CEmail, @LPhone, @CLogin, @CPassword)";
                db.Execute(query, client);
            }
        }
        private void buttonRegister_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxSurName.Text) ||
                 string.IsNullOrWhiteSpace(textBoxName.Text) ||
                string.IsNullOrWhiteSpace(textBoxPobatkovi.Text) ||
                 string.IsNullOrWhiteSpace(textBoxEmail.Text) ||
                string.IsNullOrWhiteSpace(textBoxPhone.Text) ||
                 string.IsNullOrWhiteSpace(textBoxLogin.Text) ||
                    string.IsNullOrWhiteSpace(textBoxPassword.Text))
            {
                MessageBox.Show("Будь ласка, заповніть усі поля", "Примітка", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                string surName = textBoxSurName.Text;
                string name = textBoxName.Text;
                string pobatkovi = textBoxPobatkovi.Text;
                string email = textBoxEmail.Text;
                int phone = int.Parse(textBoxPhone.Text);
                string login = textBoxLogin.Text;
                string password = textBoxPassword.Text;


                if (!Regex.IsMatch(email, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"))
                {
                    MessageBox.Show("Введіть правильну адресу електронної пошти");
                    return;
                }

                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    var existingUser = db.QueryFirstOrDefault<Client>("SELECT * FROM Client WHERE CLogin = @login", new { login });
                    if (existingUser != null)
                    {
                        MessageBox.Show("Користувач із таким логіном вже існує");
                        return;
                    }
                }

                InfoDB.Client client = new InfoDB.Client
                {
                    CSurname = surName,
                    CName = name,
                    CPobatkovi = pobatkovi,
                    CEmail = email,
                    LPhone = phone,
                    CLogin = login,
                    CPassword = password
                };

                AddUser(client);
                MessageBox.Show("Вітаємо! Ви успішно зареєструвалися в інтернет магазині Rozetka", "Успішно", MessageBoxButton.OK, MessageBoxImage.Information);
                
                this.Close();
            }
        }
    }
}
