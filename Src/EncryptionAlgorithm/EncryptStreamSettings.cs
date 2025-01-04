#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
namespace Lindexi.Src.EncryptionAlgorithm;

public struct EncryptStreamSettings
{
    public EncryptStreamSettings()
    {
    }

    /// <summary>
    /// 是否允许给密码块追加哈希值。追加哈希值可以更方便业务层判断用户输入的密码是否正确，但是会降低爆破难度
    /// </summary>
    /// 是否允许对密码块进行 MD5 计算，根据 Kelvt Value 的建议，如果添加 MD5 计算，会降低爆破难度
    /// 对一些数据，需要禁用 MD5 计算，无论输入什么都给他解密，至于是否正确，就需要自行了解明文是否符合预期
    /// 由于密码块是生成的随机密码，当前随机数在 dotnet 6 下是依赖时间，则攻击者可以同时设置时间范围，找到所有的密码块从而进行破解。在 dotnet 6 以上使用的是共享的，可能此时经过了很多次的获取。期望能够注入随机数，允许设置为加密的随机数
    public bool ShouldAppendHashToKeyBlock { get; set; } = true;


}

#endif
