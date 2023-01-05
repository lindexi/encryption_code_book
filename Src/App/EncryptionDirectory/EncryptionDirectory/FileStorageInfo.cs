using System;

namespace EncryptionDirectory;

record FileStorageInfo(string FileName, string RelativePath, DateTimeOffset LastWriteTime, long FileSize, string SHA256,
    string Path, int[] Salt)
{
}