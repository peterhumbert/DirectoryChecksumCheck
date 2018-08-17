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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Security.Cryptography;

namespace DirectoryChecksumCheck
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnChoose1_Click(object sender, RoutedEventArgs e)
        {
            lblFolder1.Content = ChooseFolder();
        }

        private void btnChoose2_Click(object sender, RoutedEventArgs e)
        {
            lblFolder2.Content = ChooseFolder();
        }

        private String ChooseFolder()
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();

            if (result.Equals(CommonFileDialogResult.Ok))
            {
                return dialog.FileName;
            }
            return null;
        }

        private void btnCompare_Click(object sender, RoutedEventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo(lblFolder1.Content.ToString());

            foreach (String s in GetDirDictionary(di).Values)
            {
                txtOutput.Text += s + "\n";
            }
        }

        private Dictionary<String,String> GetDirDictionary(DirectoryInfo di)
        {
            Dictionary<String, String> output = new Dictionary<string, string>();
            Dictionary<String, String> temp;

            if (di.GetDirectories().Length > 0)
            {
                foreach (DirectoryInfo d in di.GetDirectories())
                {
                    temp = new Dictionary<string, string>();
                    temp = GetDirDictionary(d);

                    output = output.Concat(temp).ToDictionary(x => x.Key, x => x.Value);
                }
            }

            temp = GetFileDictionary(di);

            try
            {
                output = output.Concat(temp).ToDictionary(x => x.Key, x => x.Value);
            }
            catch (System.ArgumentException)
            {
                // one or more identical duplicate files exist
                foreach (var k in temp.Keys)
                {
                    if (!output.ContainsKey(k))
                    {
                        output.Add(k, temp[k]);
                    }
                }
            }
            

            return output;
        }

        private Dictionary<String,String> GetFileDictionary(DirectoryInfo di)
        {
            Dictionary<String, String> output = new Dictionary<string, string>();

            foreach (FileInfo f in di.GetFiles())
            {
                if (f.Extension.ToLowerInvariant().Equals(".jpg"))
                {
                    output.Add(GetFileHash(f.FullName), f.FullName);
                }
            }

            return output;
        }

        private string GetFileHash(String filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }

        }
    }
}
