using System;
using System.Security.Cryptography;

namespace Lindexi.Src.EncryptionAlgorithm;

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER

class DefaultRandomNumberGenerator : IRandomNumberGenerator
{
    public DefaultRandomNumberGenerator()
    {
#if NET6_0_OR_GREATER
        _random = Random.Shared;
#else
        _random = new Random();
#endif
    }

    private readonly Random _random;

    public int GetRandomNumber(int maxValue)
    {
        return _random.Next(maxValue);
    }

    public int GetRandomNumber(int minValue, int maxValue)
    {
        return _random.Next(minValue, maxValue);
    }

    public byte GenerateFillGapByte()
    {
        return (byte) _random.Next(byte.MinValue, byte.MaxValue);
    }

    public void FillKeyBlock(Span<byte> keyBlock)
    {
        // 只有生成密码块的时候才使用安全的随机数，其他时候使用普通的随机数即可
        RandomNumberGenerator.Fill(keyBlock);
    }

    public void FillGap(Span<byte> gap)
    {
        _random.NextBytes(gap);
    }
}

#else // NET45
class DefaultRandomNumberGenerator : IRandomNumberGenerator
{
    public DefaultRandomNumberGenerator()
    {
        // 虽然 NET 45 也有 RandomNumberGenerator 类型，但需要手动释放，且 API 也不好用，那就继续使用 Random 类型。毕竟即使加上 RandomNumberGenerator 也不是提高很多安全性。只是可能通过碰撞方式猜测空白填充的数据，但空白填充数据被猜测也不能被推断，所以使用简单的 Random 类型也可以
        var random = new Random();
        _random = random;
    }

    private readonly Random _random;

    public int GetRandomNumber(int maxValue)
    {
        return GetRandomNumber(0, maxValue);
    }

    public int GetRandomNumber(int minValue, int maxValue)
    {
        return _random.Next(minValue, maxValue);
    }

    public byte GenerateFillGapByte()
    {
        return (byte) _random.Next(byte.MinValue, byte.MaxValue);
    }
}
#endif
