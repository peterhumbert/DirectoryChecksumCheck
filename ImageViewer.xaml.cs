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

namespace DirectoryChecksumCheck
{
    /// <summary>
    /// Interaction logic for ImageViewer.xaml
    /// </summary>
    public partial class ImageViewer : Window
    {
        public ImageViewer()
        {
            InitializeComponent();
            img.Source = new BitmapImage(new Uri(@"C:\Users\Peter\Pictures\IMAG0127.jpg"));
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Prev");
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Next");
        }
    }
}
