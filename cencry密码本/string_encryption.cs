using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace cencry密码本
{
    [TestClass]
    public class string_encryption
    {
        [TestMethod]
        public void encryption()
        {
            var encry = new encryption_code_book.string_encryption();
            string str ;
            string key;
            Random ran = new Random();
            key = ranstr(10, ran);
            string temp;
            for (int i = 0; i < 1024; i++)
            {
                str = ranstr(100, ran);
                temp = encry.encryption(str, key);
                Assert.AreEqual(str, encry.decryption(temp, key));
            }
        }

        private string ranstr(int count, Random ran)
        {
            StringBuilder str = new StringBuilder();
            str.Clear();
            for (int i = 0; i < count; i++)
            {
                str.Append(Convert.ToChar(ran.Next() % 2 == 0 ? ran.Next(19968, 40864) : ran.Next(33, 126)));
            }
            return str.ToString();
        }
    }
}
