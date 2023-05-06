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
            string nameGoods = textBoxNameGood.Text;
            string typeGoods = textBoxType.Text;
            string brandGoods = textBoxBrand.Text;
            string descriptionGoods = textBoxDescription.Text;
            int countGoods = int.Parse(textBoxCount.Text);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                InfoDB.Goods goods = new InfoDB.Goods
                {
                    GName = nameGoods,
                    GType = typeGoods,
                    GBrand = brandGoods,
                    GDescription = descriptionGoods,
                    GCount = countGoods,
                    GPicture = GetPictureBytes(pictureBox.Source as BitmapImage)
                };

                string sqlQuery = "INSERT INTO Goods(GName, GType, GBrand, GDescription, GCount, GPicture) " +
                        "VALUES(@GName, @GType, @GBrand, @GDescription, @GCount, @GPicture)";
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
                Goods selectedGood = (Goods)GoodsGrid.SelectedItem;
                textBoxNameGood.Text = selectedGood.GName;
                textBoxType.Text = selectedGood.GType;
                textBoxBrand.Text = selectedGood.GBrand;
                textBoxDescription.Text = selectedGood.GDescription;
                textBoxCount.Text = selectedGood.GCount.ToString();

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
}
