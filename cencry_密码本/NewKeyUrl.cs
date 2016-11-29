using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using newKey = encryption_code_book.Model.NewKeyUrl;
namespace cencry_密码本
{
    [TestClass]
    class NewKeyUrl
    {
        [TestMethod]
        public void NewKey()
        {
            //newKey.NewKey()

            StringBuilder str = new StringBuilder();
            NewKeyAccount(str);
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
            }
        }

        private void NewKeyNum(string name, string key, string url, StringBuilder str, string partition)
        {
            List<int> num = new List<int>()
            {
                6,10,15
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
            NewKey(name,key,url,num,
                true,true,true,true,
                str,partition);
        }

        private void NewKey(string name, string key, string url,
            int num,
            bool capitalKey, bool smallKey, bool numKey, bool specialCharacter,
            StringBuilder str, string partition)
        {
            str.Append(
                partition + $"capitalKey={capitalKey},smallKey={smallKey}," +
                $"numKey={numKey},specialCharacter={specialCharacter}" + "\r\n");
            string temp = newKey.NewKey(name, key, url, num, capitalKey, smallKey, numKey, specialCharacter);
            str.Append(partition + "密码" + temp + "\r\n");
        }

        //bool capitalKey, bool smallKey, bool numKey, bool specialCharacter
    }
}
