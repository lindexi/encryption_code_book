#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
namespace Lindexi.Src.EncryptionAlgorithm;

public interface IRandomNumberGenerator
{
    int GetRandomNumber(int maxValue);

    int GetRandomNumber(int minValue, int maxValue);


}


#endif
