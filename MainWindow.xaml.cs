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
            System.Diagnostics.Debug.WriteLine(ChooseFolder());
            
        }

        private void btnChoose2_Click(object sender, RoutedEventArgs e)
        {

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
    }
}
