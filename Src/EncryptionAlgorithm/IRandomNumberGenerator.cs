using System;

namespace Lindexi.Src.EncryptionAlgorithm;

/// <summary>
/// 随机数生成器
/// </summary>
public interface IRandomNumberGenerator
{
    /// <summary>
    /// 生成随机数
    /// </summary>
    /// <param name="maxValue"></param>
    /// <returns></returns>
    int GetRandomNumber(int maxValue);

    /// <summary>
    /// 生成随机数
    /// </summary>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    /// <returns></returns>
    int GetRandomNumber(int minValue, int maxValue);

    /// <summary>
    /// 生成填充空白的数据
    /// </summary>
    /// <returns></returns>
    byte GenerateFillGapByte();

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
    /// <summary>
    /// 填充密码块，创建随机密码
    /// </summary>
    /// <param name="keyBlock"></param>
    void FillKeyBlock(Span<byte> keyBlock);
#endif
}
