using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace encryption_code_book.Model
{
    public class encryption_code
    {
        public encryption_code()
        {
            
        }
        //public ObservableCollection<> 
        private readonly string hash = @"私密密码本
作者:lindexi_gd
邮箱:lindexi_gd@163.com
哈希路径:一元n次函数

encryption_code".PadRight(1024);
        private StorageFolder _folder;
    }

     
}
