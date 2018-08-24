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
        private Dictionary<string, Node> result;
        private string[] checksums;
        private Node[] files;
        private int i = 0;

        public ImageViewer(Dictionary<string, Node> result)
        {
            InitializeComponent();
            this.result = result;
            checksums = result.Keys.ToArray<string>();
            files = result.Values.ToArray<Node>();
            img.Source = new BitmapImage(new Uri(files[i].path));
            lblNumber.Content = "Image " + (i+1) + " of " + files.Length + ": " + files[i].path;
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            i--;
            if (i < 0)
                i = checksums.Length-1;

            img.Source = new BitmapImage(new Uri(files[i].path));
            lblNumber.Content = "Image " + (i+1) + " of " + files.Length + ": " + files[i].path;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            i++;
            if (i >= checksums.Length)
                i = 0;

            img.Source = new BitmapImage(new Uri(files[i].path));
            lblNumber.Content = "Image " + (i+1) + " of " + files.Length + ": " + files[i].path;
        }
    }
}
