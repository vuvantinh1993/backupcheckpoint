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
                    var file = File.Create(ural);
                    file.Close();
                }
                var str = taoFileBackUp(listUIDName);
                System.IO.File.WriteAllText(ural, str);
                File.Open(ural, FileMode.Open);
                System.Windows.MessageBox.Show("oke! đang tạo file");
            }
        }

        public string taoFileBackUp(List<UidAndName> listUiD)
        {
            var str = "<!DOCTYPE html>\n <html lang = \"en\" >\n" +
                "<head>\n <meta charset = \"UTF-8\" ><meta name = \"viewport\" content = \"width=device-width, initial-scale=1.0\"> \n" +
                "<script src = \"https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js\"></script>\n" +
                " <title> Document </title>" +
                "</head>\n";
            str = str + "<body>" +
                 listUiD.Count() + "bạn bè <br>" +
                 "<textarea id=\"userName\" rows=\"4\" cols=\"50\"> </textarea> \n" +
                 "<button onclick=\"laygiatri()\">Tìm kiếm</button>\n";

            listUiD.ForEach(x =>
           {
               str = str + "<div class=\"user\">\n";
               str = str + "<img src=\"http://graph.facebook.com/" + x.Uid + "/picture?type=large\">\n";
               str = str + "<a href=\"https://www.facebook.com/" + x.Uid + "\" target=\"_blank\" class=\"tinh\">" + x.Name + "</a>\n" +
               "</div>\n";
           });


            // ddoanj scrip
            str = str + "<script>" +
                "function laygiatri() {\n" +
                "var input = $('#userName').val();" + "\n" +
                "if (bodau(input) != '') {" + "\n" +
                "var listUserFind = input.split('\\n');" + "\n" +
                "var listUserFindTrim = [];" + "\n" +
                "for (let i = 0; i < listUserFind.length; i++) {" + "\n" +
                "listUserFindTrim.push(bodau(listUserFind[i]).toUpperCase(0));" + "\n" +
                "}" + "\n" +
                "var listUser = $('.user').attr(\"hidden\", true);" + "\n" +
                "var list = $('.user').find('.tinh');" + "\n" +
                "for (let i = 0; i < list.length; i++) {" + "\n" +
                "if (listUserFindTrim.includes(bodau(list[i].textContent).toUpperCase())) {" + "\n" +
                "listUser[i].removeAttribute('hidden')" + "\n" +
                "}" + "\n" +
                "}" + "\n" +
                "} else {" + "\n" +
                "var listUser = $('.user').removeAttr(\"hidden\");" + "\n" +
                "}" + "\n" +
                "}" + "\n";

            str = str + "function bodau(str) {" + "\n" +
                "str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, \"a\");" + "\n" +
                " str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, \"e\");" + "\n" +
                " str = str.replace(/ì|í|ị|ỉ|ĩ/g, \"i\");" + "\n" +
                "str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, \"o\");" + "\n" +
                "str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, \"u\");" + "\n" +
                "str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, \"y\");" + "\n" +
                "str = str.replace(/đ/g, \"d\");" + "\n" +
                "str = str.replace(/À|Á|Ạ|Ả|Ã|Â|Ầ|Ấ|Ậ|Ẩ|Ẫ|Ă|Ằ|Ắ|Ặ|Ẳ|Ẵ/g, \"A\");" + "\n" +
                " str = str.replace(/È|É|Ẹ|Ẻ|Ẽ|Ê|Ề|Ế|Ệ|Ể|Ễ/g, \"E\");" + "\n" +
                "str = str.replace(/Ì|Í|Ị|Ỉ|Ĩ/g, \"I\");" + "\n" +
                "str = str.replace(/Ò|Ó|Ọ|Ỏ|Õ|Ô|Ồ|Ố|Ộ|Ổ|Ỗ|Ơ|Ờ|Ớ|Ợ|Ở|Ỡ/g, \"O\");" + "\n" +
                "str = str.replace(/Ù|Ú|Ụ|Ủ|Ũ|Ư|Ừ|Ứ|Ự|Ử|Ữ/g, \"U\");" + "\n" +
                "str = str.replace(/Ỳ|Ý|Ỵ|Ỷ|Ỹ/g, \"Y\");" + "\n" +
                "str = str.replace(/Đ/g, \"D\");" + "\n" +
                "str = str.replace(/\u0300|\u0301|\u0303|\u0309|\u0323/g, \"\");" + "\n" +
                "str = str.replace(/\u02C6|\u0306|\u031B/g, \"\");" + "\n" +
                " str = str.replace(/\\s+/g, '');" + "\n" +
                "return str;" + "\n" +
                " }" + "\n" +
                " </script>" + "\n";



            str = str + "</body>\n" +
               "</html>";

            return str;
        }
    }


    public class UidAndName
    {
        public string Uid { get; set; }
        public string Name { get; set; }
    }
}
