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
            DirectoryInfo di;
            Dictionary<String, String> d1;
            Dictionary<String, String> d2;

            btnChoose1.IsEnabled = false;
            btnChoose2.IsEnabled = false;
            txtExtensions.IsEnabled = false;
            btnCompare.IsEnabled = false;
            btnDuplicates.IsEnabled = false;
            txtOutput.Clear();

            txtOutput.Text += "Compare directories\n";
            txtOutput.Text += lblFolder1.Content.ToString() + "\n";
            txtOutput.Text += lblFolder2.Content.ToString() + "\n";

            di = new DirectoryInfo(lblFolder1.Content.ToString());
            d1 = GetDirDictionary(di);

            di = new DirectoryInfo(lblFolder2.Content.ToString());
            d2 = GetDirDictionary(di);

            foreach (String k in d1.Keys)
            {
                if (!d2.Keys.Contains(k))
                {
                    txtOutput.Text += d1[k] + " : " + k + "\n";
                }
            }

            txtOutput.Text += "\n\nDONE";

            btnChoose1.IsEnabled = true;
            btnChoose2.IsEnabled = true;
            txtExtensions.IsEnabled = true;
            btnCompare.IsEnabled = true;
            btnDuplicates.IsEnabled = true;
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
                    try
                    {
                        output = output.Concat(temp).ToDictionary(x => x.Key, x => x.Value);
                    }
                    catch (System.ArgumentException)
                    {
                        foreach (var k in temp.Keys)
                        {
                            if (!output.ContainsKey(k))
                            {
                                output.Add(k, temp[k]);
                            }
                        }
                    }
                    
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

        private Dictionary<String, String> GetFileDictionary(DirectoryInfo di)
        {
            Dictionary<String, String> output = new Dictionary<string, string>();
            string[] exts = txtExtensions.Text.Split(' ');

            foreach (FileInfo f in di.GetFiles())
            {
                if (exts.Contains(f.Extension.ToLowerInvariant()))
                {
                    try
                    {
                        output.Add(GetFileHash(f.FullName), f.FullName);
                    }
                    catch (Exception)
                    {
                    }
                    
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

        private void Wrapper(bool compare)
        {
            Dictionary<String, Node> dup;

            btnChoose1.IsEnabled = false;
            btnChoose2.IsEnabled = false;
            txtExtensions.IsEnabled = false;
            btnCompare.IsEnabled = false;
            btnDuplicates.IsEnabled = false;
            txtOutput.Clear();

            if (compare)
            {

            }
            else
            {
                txtOutput.Text += "Find duplicates\n";
                txtOutput.Text += lblFolder1.Content.ToString() + "\n";
            }
            

            BackgroundWorker bgWorker = new BackgroundWorker() { WorkerReportsProgress = true };
            bgWorker.DoWork += (s, z) => {
                ArgObj a = (ArgObj)z.Argument;
                dup = GetDirDuplicates(a.di, a.exts);

                z.Result = dup;


                // Use bgWorker.ReportProgress(); to report the current progress  
            };
            bgWorker.ProgressChanged += (s, z) => {
                // Here you will be informed about progress and here it is save to change/show progress. 
                // You can access from here savely a ProgressBars or another control.  
            };
            bgWorker.RunWorkerCompleted += (s, z) => {
                // Here you will be informed if the job is done. 
                // Use this event to unlock your gui 

                dup = (Dictionary<string, Node>)z.Result;

                if (compare)
                {

                }
                else
                {
                    foreach (String k in dup.Keys)
                    {
                        Node n = dup[k];

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
            bgWorker.RunWorkerAsync(new ArgObj(new DirectoryInfo(lblFolder1.Content.ToString()),
                txtExtensions.Text.Split(' ')));
        }

        private void btnDuplicates_Click(object sender, RoutedEventArgs e)
        {
            Wrapper(false);
        }

        private Dictionary<String, Node> GetDirDuplicates(DirectoryInfo di, string[] exts)
        {
            Dictionary<String, Node> output = new Dictionary<String, Node>();
            Dictionary<String, Node> temp;

            if (di.GetDirectories().Length > 0)
            {
                foreach (DirectoryInfo d in di.GetDirectories())
                {
                    temp = GetDirDuplicates(d, exts);
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

            temp = GetFileDuplicates(di, exts);

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

        private Dictionary<String, Node> GetFileDuplicates(DirectoryInfo di, string[] exts)
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
    }
}
