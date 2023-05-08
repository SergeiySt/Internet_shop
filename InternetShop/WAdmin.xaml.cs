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

namespace InternetShop
{
    /// <summary>
    /// Interaction logic for WAdmin.xaml
    /// </summary>
    public partial class WAdmin : Window
    {
        private string connectionString;
        public WAdmin()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["ConnectionDB"].ConnectionString;
            LoadGoods();
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

            if (System.Windows.MessageBox.Show("Ви точно хочете видалити користувача", "Попередження", MessageBoxButton.YesNo, (MessageBoxImage)MessageBoxIcon.Question) == MessageBoxResult.Yes)
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
    }
}
