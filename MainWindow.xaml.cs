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
using System.ComponentModel;

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

        private string ChooseFolder()
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
            ExecutionWrapper(true);
        }

        private string GetFileHash(String filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    string output = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                    stream.Close();
                    return output;
                }
            }

        }

        private void ExecutionWrapper(bool compare)
        {
            Dictionary<String, Node> result;

            btnChoose1.IsEnabled = false;
            btnChoose2.IsEnabled = false;
            txtExtensions.IsEnabled = false;
            btnCompare.IsEnabled = false;
            btnDuplicates.IsEnabled = false;
            txtOutput.Clear();

            if (compare)
            {
                txtOutput.Text += "Compare\n";
                txtOutput.Text += lblFolder1.Content.ToString() + "\n";
                txtOutput.Text += lblFolder2.Content.ToString() + "\n";
            }
            else
            {
                txtOutput.Text += "Find duplicates\n";
                txtOutput.Text += lblFolder1.Content.ToString() + "\n";
            }
            

            BackgroundWorker bgWorker = new BackgroundWorker() { WorkerReportsProgress = false };
            bgWorker.DoWork += (s, z) => {
                ArgObj a = (ArgObj)z.Argument;

                if (compare)
                {
                    Dictionary<String, Node> d1, d2;
                    result = new Dictionary<string, Node>();

                    d1 = GetDirDictionary(a.di1, a.exts);
                    d2 = GetDirDictionary(a.di2, a.exts);

                    foreach (string k in d1.Keys)
                    {
                        if (!d2.ContainsKey(k))
                            result.Add(k, d1[k]);
                    }
                }
                else
                {
                    result = GetDirDictionary(a.di1, a.exts);
                }

                z.Result = result;
            };
            bgWorker.RunWorkerCompleted += (s, z) => {
                // Here you will be informed if the job is done. 
                // Use this event to unlock your gui 

                result = (Dictionary<string, Node>)z.Result;

                if (compare)
                {
                    txtOutput.Text += "\n";
                    foreach (string k in result.Keys)
                        txtOutput.Text += result[k].path + " : " + k + "\n";          
                }
                else
                {
                    foreach (String k in result.Keys)
                    {
                        Node n = result[k];

                        if (n.hasNext)
                        {
                            txtOutput.Text += "\n" + k + "\n";
                            txtOutput.Text += n.path + "\n";

                            while (n.hasNext)
                            {
                                n = n.getNext();
                                txtOutput.Text += n.path + "\n";
                            }
                        }
                    }
                }
                

                txtOutput.Text += "\n\nDONE";

                btnChoose1.IsEnabled = true;
                btnChoose2.IsEnabled = true;
                txtExtensions.IsEnabled = true;
                btnCompare.IsEnabled = true;
                btnDuplicates.IsEnabled = true;
            };

            if (compare)
            {
                bgWorker.RunWorkerAsync(new ArgObj(new DirectoryInfo(lblFolder1.Content.ToString()),
                    new DirectoryInfo(lblFolder2.Content.ToString()),
                    txtExtensions.Text.Split(' ')));
            }
            else
            {
                bgWorker.RunWorkerAsync(new ArgObj(new DirectoryInfo(lblFolder1.Content.ToString()),
                    txtExtensions.Text.Split(' ')));
            }
            
        }

        private void btnDuplicates_Click(object sender, RoutedEventArgs e)
        {
            ExecutionWrapper(false);
        }

        private Dictionary<String, Node> GetDirDictionary(DirectoryInfo di, string[] exts)
        {
            Dictionary<String, Node> output = new Dictionary<String, Node>();
            Dictionary<String, Node> temp;

            if (di.GetDirectories().Length > 0)
            {
                foreach (DirectoryInfo d in di.GetDirectories())
                {
                    temp = GetDirDictionary(d, exts);
                    try
                    {
                        output = output.Concat(temp).ToDictionary(x => x.Key, x => x.Value);
                    }
                    catch (System.ArgumentException)
                    {
                        foreach (var k in temp.Keys)
                        {
                            if (!output.ContainsKey(k))
                                output.Add(k, temp[k]);
                            else
                            {
                                Node n = output[k];

                                while (n.hasNext)
                                    n = n.getNext();

                                n.setNext(temp[k]);
                            }
                        }
                    }
                }
            }

            temp = GetFileDictionary(di, exts);

            try
            {
                output = output.Concat(temp).ToDictionary(x => x.Key, x => x.Value);
            }
            catch (System.ArgumentException)
            {
                // one or more identical/duplicate checksums exist
                foreach (var k in temp.Keys)
                {
                    if (!output.ContainsKey(k))
                        output.Add(k, temp[k]);
                    else
                    {
                        Node n = output[k];

                        while (n.hasNext)
                            n = n.getNext();

                        n.setNext(temp[k]);
                    }
                }
            }


            return output;
        }

        private Dictionary<String, Node> GetFileDictionary(DirectoryInfo di, string[] exts)
        {
            Dictionary<String, Node> output = new Dictionary<string, Node>();

            foreach (FileInfo f in di.GetFiles())
            {
                if (exts.Contains(f.Extension.ToLowerInvariant()))
                {
                    string k = GetFileHash(f.FullName);
                    try
                    {
                        output.Add(k, new Node(f.FullName));
                    }
                    catch (Exception)
                    {
                        Node n = new Node(f.FullName, output[k]);
                        output[k] = n;
                    }

                }
            }

            return output;
        }

        private void btnOpenImgViewer_Click(object sender, RoutedEventArgs e)
        {
            ImageViewer imageViewer = new ImageViewer();
            imageViewer.Show();
        }
    }
}
