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
using System.IO;
using System.Windows.Forms;
using System.Data;

namespace InternetShop
{
    /// <summary>
    /// Interaction logic for WUser.xaml
    /// </summary>
    public partial class WUser : Window
    {
        private string connectionString;

        private string surName = "";
        private string Name = "";
        private string pobatkovi = "";
        public WUser(string surName, string Name, string pobatkovi)
        {
            InitializeComponent();
            DataContext = this;
            connectionString = ConfigurationManager.ConnectionStrings["ConnectionDB"].ConnectionString;
            LoadGoods();

            this.surName = surName;
            this.Name = Name;
            this.pobatkovi= pobatkovi;
            FIO.Text = $"{surName} {Name} {pobatkovi}";

            GoodsGrid2.IsReadOnly = true;
        }

        private void LoadGoods()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Goods";
                List<Goods> goods = connection.Query<Goods>(query).ToList();
                GoodsGrid2.ItemsSource = goods;

            }
        }
        private void GoodsGrid_SelectionChanged2(object sender, SelectionChangedEventArgs e)
        {
            textBoxCount.Text = "1";
            int count = int.Parse(textBoxCount.Text);

            if (GoodsGrid2.SelectedItem != null)
            {
                if (GoodsGrid2.SelectedItem is Goods selectedGood)
                {
                    textBoxNameTovar.Text = selectedGood.GName;

                    textBlockSumm.Text = $"{selectedGood.GPrice} $";

                    byte[] bytes = selectedGood.GPicture;
                    if (bytes != null && bytes.Length > 0)
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = new MemoryStream(bytes);
                        bitmap.EndInit();
                        pictureBox2.Source = bitmap;
                    }
                }
            }
        }

        private void textBoxCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            decimal count;
            if (decimal.TryParse(textBoxCount.Text, out count))
            {
                decimal price;
                if (decimal.TryParse(textBlockSumm.Text.Replace("$", ""), out price))
                {
                    textBlockSumm.Text = $"{price * count} $";
                }
            }
        }

        private void buttonOrder_Click(object sender, RoutedEventArgs e)
        {

            Random random = new Random();
            int randomNumber = random.Next(100000, 999999);

            int numberOrder = randomNumber - 1;
            string nameTovar = textBoxNameTovar.Text;
            //decimal countTovar = decimal.Parse(textBoxCount.Text);
            decimal countTovar;
            bool countParsed = decimal.TryParse(textBoxCount.Text, out countTovar);
            string adress = textBoxAdress.Text;
            string fio = FIO.Text;
            //decimal price = decimal.Parse(textBlockSumm.Text.Replace("$", ""));
            decimal price;
            bool priceParsed = decimal.TryParse(textBlockSumm.Text.Replace("$", ""), out price);
            bool chek = false;

            if (string.IsNullOrEmpty(nameTovar))
            {
                System.Windows.MessageBox.Show("Поле 'Назва товару' не заповнено", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!countParsed || countTovar <= 0)
            {
                System.Windows.MessageBox.Show("Поле 'Кількість товару' не заповнено або заповнено некоректно", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(textBoxAdress.Text))
            {
                System.Windows.MessageBox.Show("Поле 'Адреса доставки' не заповнене", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                InfoDB.GoodsOrder goodsOrder = new InfoDB.GoodsOrder
                {
                    ONumberOrder = numberOrder,
                    ONameGoods = nameTovar,
                    OCount = int.Parse(textBoxCount.Text),
                    OAdress = adress,
                    OFIO = fio,
                    OPrice = price,
                    OStatus = chek
                };

                string sqlQuery = "INSERT INTO GoodsOrder(ONumberOrder, ONameGoods, OCount, OAdress, OFIO, OPrice, OStatus) " +
                    "VALUES(@ONumberOrder, @ONameGoods, @OCount, @OAdress, @OFIO, @OPrice, @OStatus)";
                connection.Execute(sqlQuery, goodsOrder);
            }

            ClearText();

            System.Windows.MessageBox.Show($" Номер вашого замовлення: {numberOrder}\n" +
                $"Назва товару: {nameTovar}\n" +
                $"Загальна сума: {price}\n" +
                $"\n" +
                $"Дякую вам за покупку!", "Замовлення", MessageBoxButton.OK, (MessageBoxImage)MessageBoxIcon.Information);
        }

        private void ClearText()
        {
            textBoxNameTovar.Text = " ";
            textBoxCount.Text = " ";
            textBoxAdress.Text = " ";
            textBlockSumm.Text = " ";
            pictureBox2.Source = null;
        }

        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
            
        }
    }
}
