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
using System.Data;
using System.IO;
using static InternetShop.InfoDB;
using System.Windows.Forms;
using System.Net.PeerToPeer;

namespace InternetShop
{
    /// <summary>
    /// Interaction logic for WAdmin.xaml
    /// </summary>
    public partial class WAdmin : Window
    {
        private string connectionString;

        private string surName = "";
        private string Name = "";
        private string pobatkovi = "";

        public WAdmin(string surName, string Name, string pobatkovi)
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["ConnectionDB"].ConnectionString;
            LoadGoods();
            LoadGoodsClient();
            LoadGoodsOrder();
            LoadPerson();
            this.surName = surName;
            this.Name = Name;
            this.pobatkovi = pobatkovi;

            textBlockPerson.Text = $"{surName} {Name} {pobatkovi}";
        }

        private void buttonAddGood_Click(object sender, RoutedEventArgs e)
        {
          

            if (string.IsNullOrWhiteSpace(textBoxNameGood.Text))
            {
                System.Windows.MessageBox.Show("Введіть назву товару", "Примітка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(textBoxType.Text))
            {
                System.Windows.MessageBox.Show("Введіть тип товару", "Примітка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(textBoxBrand.Text))
            {
                System.Windows.MessageBox.Show("Введіть бренд товару", "Примітка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(textBoxPrice.Text))
            {
                System.Windows.MessageBox.Show("Введіть ціну товару", "Примітка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(textBoxCount.Text))
            {
                System.Windows.MessageBox.Show("Введіть кількість товару", "Примітка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (pictureBox.Source == null)
            {
                System.Windows.MessageBox.Show("Додати зображення товару", "Примітка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string nameGoods = textBoxNameGood.Text;
            string typeGoods = textBoxType.Text;
            string brandGoods = textBoxBrand.Text;
            decimal price = decimal.Parse(textBoxPrice.Text);
            string descriptionGoods = textBoxDescription.Text;
            int countGoods = int.Parse(textBoxCount.Text);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                InfoDB.Goods goods = new InfoDB.Goods
                {
                    GName = nameGoods,
                    GType = typeGoods,
                    GBrand = brandGoods,
                    GPrice = price,
                    GDescription = descriptionGoods,
                    GCount = countGoods,
                    GPicture = GetPictureBytes(pictureBox.Source as BitmapImage)
                };

                string sqlQuery = "INSERT INTO Goods(GName, GType, GBrand, GPrice, GDescription, GCount, GPicture) " +
                        "VALUES(@GName, @GType, @GBrand, @GPrice, @GDescription, @GCount, @GPicture)";
                connection.Execute(sqlQuery, goods);
            }
            LoadGoods();
        }
        private byte[] GetPictureBytes(BitmapImage image)
        {
            if (image == null) return null;

            using (MemoryStream stream = new MemoryStream())
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(stream);
                return stream.ToArray();
            }
        }
        private void LoadGoods()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Goods";
                List<Goods> goods = connection.Query<Goods>(query).ToList();
                GoodsGrid.ItemsSource = goods;
               
            }
        }
        private void LoadGoodsClient()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query2 = "SELECT * FROM Client";
                List<Client> client = connection.Query<Client>(query2).ToList();
                UsersGrid.ItemsSource = client;

            }
        }

       

        private void LoadGoodsOrder()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query2 = "SELECT * FROM GoodsOrder";
                List<GoodsOrder> order = connection.Query<GoodsOrder>(query2).ToList();
                OrderGrid.ItemsSource = order;

            }
        }

        private void LoadPerson()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query2 = "SELECT * FROM Personnel";
                List<Personnel> order = connection.Query<Personnel>(query2).ToList();
                PersonGrid.ItemsSource = order;

            }
        }
        private void buttonAddPicture_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "JPEG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png|GIF Files (*.gif)|*.gif|All Files (*.*)|*.*";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                pictureBox.Source = new BitmapImage(new Uri(filename));
            }
        }

        private void GoodsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GoodsGrid.SelectedItem != null)
            {
                if(GoodsGrid.SelectedItem is Goods selectedGood)
                {
                    textBoxNameGood.Text = selectedGood.GName;
                    textBoxType.Text = selectedGood.GType;
                    textBoxBrand.Text = selectedGood.GBrand;
                    textBoxDescription.Text = selectedGood.GDescription;
                    textBoxCount.Text = selectedGood.GCount.ToString();
                    textBoxPrice.Text = selectedGood.GPrice.ToString();

                    byte[] bytes = selectedGood.GPicture;
                    if (bytes != null && bytes.Length > 0)
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = new MemoryStream(bytes);
                        bitmap.EndInit();
                        pictureBox.Source = bitmap;
                    }
                }
            }
        }

        private void buttonDeleteGood_Click(object sender, RoutedEventArgs e)
        {
            if(GoodsGrid.SelectedItems.Count == 0)
            {
               System.Windows.MessageBox.Show("Виберіть запис для видалення.", "Error", MessageBoxButton.OK, (MessageBoxImage)MessageBoxIcon.Error);
                return;
            }

            if (System.Windows.MessageBox.Show("Ви точно хочете видалити товар", "Попередження", MessageBoxButton.YesNo, (MessageBoxImage)MessageBoxIcon.Question) == MessageBoxResult.Yes)
            {
                if (GoodsGrid.SelectedItem != null)
                {
                    int id = ((InfoDB.Goods)GoodsGrid.SelectedItem).id_goods;

                    using (IDbConnection connection = new SqlConnection(connectionString))
                    {
                        string sqlQuery = "DELETE FROM Goods WHERE id_goods = @id_goods";
                        connection.Execute(sqlQuery, new { id_goods = id });
                        System.Windows.MessageBox.Show("Успішно видалено");
                    }

                    ClearFields();
                    LoadGoods();
                    
                }
            }
        }

        private void ClearFields()
        {
            textBoxNameGood.Text = " ";
            textBoxType.Text = " ";
            textBoxBrand.Text = " ";
            textBoxDescription.Text = " ";
            textBoxCount.Text = " ";
            textBoxPrice.Text = " ";
            pictureBox.Source = null;
        }

        private bool EditGoods()
        {

            if (string.IsNullOrWhiteSpace(textBoxNameGood.Text))
            {
                System.Windows.MessageBox.Show("Введіть назву товару", "Примітка", MessageBoxButton.OK, MessageBoxImage.Information);
              return false;
            }

            else if (string.IsNullOrWhiteSpace(textBoxType.Text))
            {
                System.Windows.MessageBox.Show("Введіть тип товару", "Примітка", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            else if(string.IsNullOrWhiteSpace(textBoxBrand.Text))
            {
                System.Windows.MessageBox.Show("Введіть бренд товару", "Примітка", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
            else if (string.IsNullOrEmpty(textBoxDescription.Text))
            {
                System.Windows.MessageBox.Show("Введіть опис товару", "Примітка", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            else if(string.IsNullOrWhiteSpace(textBoxPrice.Text))
            {
                System.Windows.MessageBox.Show("Введіть ціну товару", "Примітка", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            else if(string.IsNullOrWhiteSpace(textBoxCount.Text))
            {
                System.Windows.MessageBox.Show("Введіть кількість товару", "Примітка", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            else if(pictureBox.Source == null)
            {
                System.Windows.MessageBox.Show("Додати зображення товару", "Примітка", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
            else
            {
                return true;
            }
        }
        private void buttonUpdateGood_Click(object sender, RoutedEventArgs e)
        {
            if(GoodsGrid.SelectedItem != null)
            {
                InfoDB.Goods selectedGood = GoodsGrid.SelectedItem as InfoDB.Goods;
                if (selectedGood != null)
                {
                    int id = selectedGood.id_goods; // получаем id из объекта

                    if (EditGoods())
                    {
                        string nameGoods = textBoxNameGood.Text;
                        string typeGoods = textBoxType.Text;
                        string brandGoods = textBoxBrand.Text;
                        decimal price = decimal.Parse(textBoxPrice.Text);
                        string descriptionGoods = textBoxDescription.Text;
                        int countGoods = int.Parse(textBoxCount.Text);


                        byte[] pictureBytes = GetPictureBytes(pictureBox.Source as BitmapImage);

                        using (IDbConnection connection = new SqlConnection(connectionString))
                        {
                            string sqlQuery = "UPDATE Goods SET GName = @GName, GType = @GType, GBrand = @GBrand, GPrice = @GPrice, " +
                                              "GDescription = @GDescription, GCount = @GCount, GPicture = @GPicture WHERE id_goods = @id_goods";
                            connection.Execute(sqlQuery, new
                            {
                                GName = nameGoods,
                                GType = typeGoods,
                                GBrand = brandGoods,
                                GPrice = price,
                                GDescription = descriptionGoods,
                                GCount = countGoods,
                                GPicture = pictureBytes,

                                id_goods = id
                            });
                        }
                        LoadGoods();
                        ClearFields();
                    }
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Неможливо оновити запис. Перевірте, чи всі поля заповнені та вибрано запис в таблиці.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PersonGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PersonGrid.SelectedItem != null)
            {
                if (PersonGrid.SelectedItem is Personnel selectedPerson)
                {
                    textSurNamePerson.Text = selectedPerson.PSurname;
                    textNamePerson.Text = selectedPerson.PName;
                    textPobatkoviPerson.Text = selectedPerson.PPobatkovi;
                    textLoginPerson.Text = selectedPerson.PLogin;
                    textPasswordPerson.Text = selectedPerson.PPassword;
                    textRolePerson.Text = selectedPerson.PRole;
                }
            }
        }

        public void ClearPerson()
        {
            textSurNamePerson.Text = "";
            textNamePerson.Text = "";
            textPobatkoviPerson.Text = "";
            textLoginPerson.Text = "";
            textPasswordPerson.Text = "";
            textRolePerson.Text = "";
        }

        private bool EditPerson()
        {

            if (string.IsNullOrWhiteSpace(textSurNamePerson.Text))
            {
                System.Windows.MessageBox.Show("Введіть прізвище співробітника", "Примітка", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            else if (string.IsNullOrWhiteSpace(textNamePerson.Text))
            {
                System.Windows.MessageBox.Show("Введіть ім'я співробітника", "Примітка", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            else if (string.IsNullOrWhiteSpace(textPobatkoviPerson.Text))
            {
                System.Windows.MessageBox.Show("Введіть по-батькові співробітника", "Примітка", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            else if (string.IsNullOrWhiteSpace(textLoginPerson.Text))
            {
                System.Windows.MessageBox.Show("Введіть логін співробітника", "Примітка", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            else if (string.IsNullOrWhiteSpace(textPasswordPerson.Text))
            {
                System.Windows.MessageBox.Show("Введіть пароль співробітника", "Примітка", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            else if (string.IsNullOrWhiteSpace(textRolePerson.Text))
            {
                System.Windows.MessageBox.Show("Додати роль співробітника", "Примітка", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
            else
            {
                return true;
            }
        }
        private void buttonAddPerson_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textSurNamePerson.Text))
            {
                System.Windows.MessageBox.Show("Введіть прізвище співробітника", "Примітка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(textNamePerson.Text))
            {
                System.Windows.MessageBox.Show("Введіть ім'я співробітника", "Примітка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(textPobatkoviPerson.Text))
            {
                System.Windows.MessageBox.Show("Введіть по-батькові співробітника", "Примітка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(textLoginPerson.Text))
            {
                System.Windows.MessageBox.Show("Введіть логін співробітника", "Примітка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(textPasswordPerson.Text))
            {
                System.Windows.MessageBox.Show("Введіть пароль співробітника", "Примітка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(textRolePerson.Text))
            {
                System.Windows.MessageBox.Show("Додати роль співробітника", "Примітка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string surnamePerson = textSurNamePerson.Text;
            string namePerson = textNamePerson.Text;
            string pobatkoviPerson = textPobatkoviPerson.Text;
            string loginPerson = textLoginPerson.Text;
            string passwordPerson = textPasswordPerson.Text;
            string rolePerson = textRolePerson.Text;


            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var existingPerson = db.QueryFirstOrDefault<Personnel>("SELECT * FROM Personnel WHERE PLogin = @loginPerson", new { loginPerson });
                if (existingPerson != null)
                {
                    System.Windows.MessageBox.Show("Cпівробітник із таким логіном вже існує", "Примітка", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                InfoDB.Personnel personnel = new InfoDB.Personnel()
                {
                    PSurname = surnamePerson,
                    PName = namePerson,
                    PPobatkovi = pobatkoviPerson,
                    PLogin = loginPerson,
                    PPassword = passwordPerson,
                    PRole = rolePerson,
                    
                };

                string sqlQuery = "INSERT INTO Personnel(PSurname, PName, PPobatkovi, PLogin, PPassword, PRole)" +
                    "VALUES (@PSurname, @PName, @PPobatkovi, @PLogin, @PPassword, @PRole)";
                connection.Execute(sqlQuery, personnel);
            }
            ClearPerson();
            LoadPerson();
        }

        private void buttonUpdatePerson_Click(object sender, RoutedEventArgs e)
        {
            if (PersonGrid.SelectedItem != null)
            {
                InfoDB.Personnel selectedPerson = PersonGrid.SelectedItem as InfoDB.Personnel;
                if (selectedPerson != null)
                {
                    int id = selectedPerson.id_personnel; // получаем id из объекта

                    if (EditPerson())
                    {
                        string surnamePerson = textSurNamePerson.Text;
                        string namePerson = textNamePerson.Text;
                        string pobatkoviPerson = textPobatkoviPerson.Text;
                        string loginPerson = textLoginPerson.Text;
                        string passwordPerson = textPasswordPerson.Text;
                        string rolePerson = textRolePerson.Text;


                        using (IDbConnection connection3 = new SqlConnection(connectionString))
                        {
                            string sqlQuery = "UPDATE Personnel SET PSurname = @PSurname, PName = @PName, PPobatkovi = @PPobatkovi, PLogin = @PLogin, " +
                                              "PPassword = @PPassword, PRole = @PRole WHERE id_personnel = @id_personnel";
                            connection3.Execute(sqlQuery, new
                            {
                                PSurname = surnamePerson,
                                PName = namePerson,
                                PPobatkovi = pobatkoviPerson,
                                PLogin = loginPerson,
                                PPassword = passwordPerson,
                                PRole = rolePerson,

                                id_personnel = id
                            });
                        }
                        ClearPerson();
                        LoadPerson();
                    }
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Неможливо оновити запис. Перевірте, чи всі поля заповнені та вибрано запис в таблиці.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void buttonDeletePerson_Click(object sender, RoutedEventArgs e)
        {
            if (PersonGrid.SelectedItems.Count == 0)
            {
                System.Windows.MessageBox.Show("Виберіть запис для видалення.", "Error", MessageBoxButton.OK, (MessageBoxImage)MessageBoxIcon.Error);
                return;
            }

            if (System.Windows.MessageBox.Show("Ви точно хочете видалити співробітника", "Попередження", MessageBoxButton.YesNo, (MessageBoxImage)MessageBoxIcon.Question) == MessageBoxResult.Yes)
            {
                if (PersonGrid.SelectedItem != null)
                {
                    int id = ((InfoDB.Personnel)PersonGrid.SelectedItem).id_personnel;

                    using (IDbConnection connection = new SqlConnection(connectionString))
                    {
                        string sqlQuery = "DELETE FROM Personnel WHERE id_personnel = @id_personnel";
                        connection.Execute(sqlQuery, new { id_personnel = id });
                        System.Windows.MessageBox.Show("Успішно видалено");
                    }

                    ClearPerson();
                    LoadPerson();

                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
           
        }
    }
}
