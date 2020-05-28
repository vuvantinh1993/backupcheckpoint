using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace backupckeckpoint
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
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // mo thu muc them file
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = false;
            fileDialog.Filter = "File| *.txt";
            fileDialog.DefaultExt = "*.txt";
            fileDialog.ShowDialog();
            if (!string.IsNullOrWhiteSpace(fileDialog.FileName))
            {
                txtimport.Text = fileDialog.FileNames[0];
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (File.Exists(txtimport.Text))
            {
                string text = File.ReadAllText(txtimport.Text);
                MatchCollection matchList = Regex.Matches(text, @"(id\=\d{15})|(el.{5,20}role)");
                var list = matchList.Cast<Match>().Select(match => match.Value).ToList();

                var listUIDName = new List<UidAndName>();

                for (int i = 0; i < list.Count(); i++)
                {
                    if (list[i].Contains("id="))
                    {
                        var UIDAndName = new UidAndName();
                        for (int j = i + 1; j < list.Count(); j++)
                        {
                            if (list[j].Contains("el="))
                            {
                                UIDAndName.Uid = list[j - 1].Replace("id=", "");
                                var a = list[j].Replace("el=\"", "");
                                UIDAndName.Name = a.Replace("\" role", "");
                                listUIDName.Add(UIDAndName);
                                i = j;
                                break;
                            }
                        }
                    }
                }

                // ghi file
                var name = DateTime.UtcNow.ToString("dd_MM_yyyy");
                string FolderName = System.IO.Path.GetDirectoryName(txtimport.Text);
                string ural = $"{FolderName}\\{name}.html";

                if (!File.Exists(ural))
                {
                    File.Create(ural);
                }
                System.IO.File.WriteAllText(ural, listUIDName.ToString());
            }
        }
    }


    public class UidAndName
    {
        public string Uid { get; set; }
        public string Name { get; set; }
    }
}
