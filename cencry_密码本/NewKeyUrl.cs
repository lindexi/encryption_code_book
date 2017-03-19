using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using newKey = encryption_code_book.Model.NewKeyUrl;

namespace cencry_密码本
{
    [TestClass]
    public class NewKeyUrl
    {
        [TestMethod]
        public async Task NewKey()
        {
            //newKey.NewKey()

            StringBuilder str = new StringBuilder();

            //NewKeyAccount(str);

            string x = "lindexi",
                x1 = "123456",
                x2 = "csdn.net";
            str.Append(newKey.Md5(x) + "\r\n");
            str.Append(newKey.Md5(x1) + "\r\n");
            str.Append(newKey.Md5(x2) + "\r\n");



            //str.Append(newKey.HaKey(x, x1, x2));

            //new DebugSettings().
            //Debug.Write(str.ToString());
            await Storage(str.ToString());
        }

        private async Task Storage(string str)
        {
            //FileSavePicker pick=new FileSavePicker();

            //pick.FileTypeChoices.Add("txt",new List<string>() { ".txt"});

            StorageFolder folder = KnownFolders.PicturesLibrary;

            StorageFile file = await folder.CreateFileAsync("1.txt");

            await FileIO.WriteTextAsync(file, str);
        }

        private void NewKeyAccount(StringBuilder str)
        {
            string partition = "";
            List<string> name = new List<string>()
            {
                "lindexi",
                "csdn",
                "a",
                ""
            };
            foreach (var temp in name)
            {
                str.Append("账户名：" + temp + "\r\n");
                NewKeyAccount(temp, str, partition);
            }
        }

        private void NewKeyAccount(string name, StringBuilder str, string partition)
        {
            List<string> key = new List<string>()
            {
                "123456",
                "a123456",
                "lin_123",
                ""
            };

            partition += " ";

            foreach (var temp in key)
            {
                str.Append(partition + "密码:" + temp + "\r\n");
                NewKeyAccount(name, temp, str, partition);
            }
        }

        private void NewKeyAccount(string name, string key, StringBuilder str, string partition)
        {
            List<string> url = new List<string>()
            {
                "http://unity.codeplex.com/",
                "csdn.net",
                "u.csdn.net",
                "./encry"
            };

            partition += " ";

            foreach (var temp in url)
            {
                str.Append(partition + "url:" + temp + "\r\n");
                NewKeyNum(name, key, temp, str, partition);
                //str.Append($"has:{newKey.HaKey(name, key, temp)}\r\n");
            }
        }

        private void NewKeyNum(string name, string key, string url, StringBuilder str, string partition)
        {
            List<int> num = new List<int>()
            {
                6,
                10,
                15
            };

            partition += " ";

            foreach (var temp in num)
            {
                str.Append(partition + "num:" + temp + "\r\n");
                NewKeyCapital(name, key, url, temp, str, partition);
            }
        }

        private void NewKeyCapital(string name, string key,
            string url, int num, StringBuilder str, string partition)
        {
            partition += " ";
            //bool capitalKey = true;
            //bool smallKey = true;
            //bool numKey = true;
            //bool specialCharacter = true;
            for (int i = 1; i < 17; i++)
            {
                NewKey(name, key, url, num,
                    (i & 1) > 0, (i & 2) > 0, (i & 4) > 0, (i & 8) > 0,
                    str, partition);
            }

            //for (int i = 0; i < 17; i++)
            //{
            //    bool x1, x2, x3, x4;

            //    x1 = (i & 1) > 0;
            //    x2 = (i & 2) > 0;
            //    x3 = (i & 4) > 0;
            //    x4 = (i & 8) > 0;

            //    string temp = "";
            //    //temp= $"x1={x1},x2={x2},x3={x3},x4={x4}"
            //    temp = (x1 ? 0.ToString() : 1.ToString()) + "," +
            //           (x2 ? 0.ToString() : 1.ToString()) + "," +
            //           (x3 ? 0.ToString() : 1.ToString()) + "," +
            //           (x4 ? 0.ToString() : 1.ToString()) ;
            //    System.Console.WriteLine(temp);
            //}
        }

        private void NewKey(string name, string key, string url,
            int num,
            bool capitalKey, bool smallKey, bool numKey, bool specialCharacter,
            StringBuilder str, string partition)
        {
            str.Append(
                partition + $"capitalKey={capitalKey},smallKey={smallKey}," +
                $"numKey={numKey},specialCharacter={specialCharacter}" + "\r\n");
            try
            {
                string temp = newKey.NewKey(name, key, url, num, capitalKey, smallKey, numKey, specialCharacter);
                str.Append(partition + "密码" + temp + "\r\n");
            }
            catch (Exception e)
            {
                str.Append(partition + e.ToString());
            }
        }

        //bool capitalKey, bool smallKey, bool numKey, bool specialCharacter
    }
}
