// lindexi
// 18:08

#region

using System;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

#endregion

namespace cencry密码本
{
    [TestClass]
    public class string_encryption
    {
        [TestMethod]
        //public void encryption()
        //{
        //    encryption_code_book.Model.string_encryption encry = new encryption_code_book.Model.string_encryption();
        //    string str;
        //    string key;
        //    Random ran = new Random();
        //    //随机密钥
        //    key = ranstr(10, ran);
        //    string temp;
        //    for (int i = 0; i < 10; i++)
        //    {
        //        //随机字符串
        //        str = ranstr(100, ran);
        //        temp = encry.encryption(str, key);
        //        Assert.AreEqual(str, encry.decryption(temp, key));
        //    }

        //    //随机错误密码
        //}

        private string ranstr(int count, Random ran)
        {
            StringBuilder str = new StringBuilder();
            str.Clear();
            for (int i = 0; i < count; i++)
            {
                str.Append(Convert.ToChar(ran.Next()%2 == 0 ? ran.Next(19968, 40864) : ran.Next(33, 126)));
            }
            return str.ToString();
        }
    }
}