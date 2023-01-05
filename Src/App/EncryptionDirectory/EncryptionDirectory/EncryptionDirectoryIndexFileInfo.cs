using System.Collections.Generic;

namespace EncryptionDirectory;

record EncryptionDirectoryIndexFileInfo(string HeaderText, List<FileStorageInfo> FileStorageInfoList)
{
    public const string DefaultHeaderText = "EncryptionDirectory 1.0";
}