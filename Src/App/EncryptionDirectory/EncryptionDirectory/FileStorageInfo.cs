using System;

namespace EncryptionDirectory;

/// <summary>
/// 文件存档信息
/// </summary>
/// <param name="FileName">文件名</param>
/// <param name="RelativePath">相对于原有文件夹的路径</param>
/// <param name="LastWriteTime">最后一次写入的时间</param>
/// <param name="FileSize">文件大小</param>
/// <param name="SHA256">文件的 SHA256 值</param>
/// <param name="Path">相对于加密文件夹的路径</param>
/// <param name="Salt">加密此文件的信息</param>
record FileStorageInfo(string FileName, string RelativePath, DateTimeOffset LastWriteTime, long FileSize, string SHA256,
    string Path, int[] Salt)
{
}
