using System;
using System.Collections.Generic;
using System.Text;

namespace Lindexi.Src.EncryptionAlgorithm
{
    // 本文件存放的是修复了 dataValue = (byte) (dataValue + hashData.KeyValue); 将构成哈希下标的密码加入了密文计算里面的版本。由于一旦改了此版本内容会导致之前加密的内容无法正确解密，于是就升级了此版本，再写一套好了
    public static partial class BinaryEncryption
    {
    }
}
