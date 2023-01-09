using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Lindexi.Src.EncryptionAlgorithm;

namespace EncryptionDirectory;

public class DirectoryEncryption
{
    public DirectoryEncryption(int[] key, DirectoryInfo sourceDirectory, DirectoryInfo targetDirectory)
    {
        Key = key;
        SourceDirectory = sourceDirectory;
        TargetDirectory = targetDirectory;
    }

    public int[] Key { get; }
    public DirectoryInfo SourceDirectory { get; }
    public DirectoryInfo TargetDirectory { get; }

    public async Task DecryptDirectoryAsync(DirectoryInfo outputDirectory)
    {
        var indexFile = Path.Join(TargetDirectory.FullName, EncryptionDirectoryIndexFileName);

        if (File.Exists(indexFile))
        {
            var encryptionDirectoryIndexFileInfo = await DecryptIndexFile(indexFile);

            foreach (var fileStorageInfo in encryptionDirectoryIndexFileInfo.FileStorageInfoList)
            {
                var filePath = Path.Join(outputDirectory.FullName, fileStorageInfo.RelativePath);

                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                if (fileStorageInfo.FileSize == 0)
                {
                    // 没有长度的文件，没有被加密
                    await File.Create(filePath).DisposeAsync();
                    File.SetLastWriteTimeUtc(filePath, fileStorageInfo.LastWriteTime.UtcDateTime);

                    continue;
                }

                var path = Path.Join(TargetDirectory.FullName, fileStorageInfo.Path);

                var key = Key.Concat(fileStorageInfo.Salt).ToArray();

                var encryptionFileByteList = await File.ReadAllBytesAsync(path);

                var fileByteList = BinaryEncryption.Decrypt(encryptionFileByteList, key, suffixData: SuffixData);

                if (fileByteList is null)
                {
                    throw new KeyErrorEncryptionDirectoryException();
                }

                // 判断一下 SHA256 是否正确
                var sha256 = SHA256.HashData(fileByteList);
                var hash = ByteListToString(sha256);

                if (!fileStorageInfo.SHA256.Equals(hash, StringComparison.Ordinal))
                {
                    throw new FileDamageEncryptionDirectoryException();
                }

                await File.WriteAllBytesAsync(filePath, fileByteList);

                File.SetLastWriteTimeUtc(filePath, fileStorageInfo.LastWriteTime.UtcDateTime);
            }
        }
    }

    public async Task UpdateAsync(IProgress<UpdateProgress>? progress = null)
    {
        var indexFile = Path.Join(TargetDirectory.FullName, EncryptionDirectoryIndexFileName);
        var packageDirectory = Directory.CreateDirectory(Path.Join(TargetDirectory.FullName, "Package"));

        var currentFileStorageInfoDictionary = new Dictionary<string /*RelativePath*/, FileStorageInfo>();

        if (File.Exists(indexFile))
        {
            var encryptionDirectoryIndexFileInfo = await DecryptIndexFile(indexFile);

            foreach (var fileStorageInfo in encryptionDirectoryIndexFileInfo.FileStorageInfoList)
            {
                currentFileStorageInfoDictionary.Add(fileStorageInfo.RelativePath, fileStorageInfo);
            }
        }

        var fileStorageInfoList = new List<FileStorageInfo>();

        var fileList = SourceDirectory.GetFiles("*", SearchOption.AllDirectories);
        for (var i = 0; i < fileList.Length; i++)
        {
            var source = fileList[i];
            var relativePath = Path.GetRelativePath(SourceDirectory.FullName, source.FullName);

            progress?.Report(new UpdateProgress(source.Name, i, relativePath, fileList.Length));

            if (currentFileStorageInfoDictionary.TryGetValue(relativePath, out var fileStorageInfo))
            {
                // 已经存放了，那就判断一下文件是否改变，是否需要更新
                if (source.Length == fileStorageInfo.FileSize &&
                    source.LastWriteTimeUtc == fileStorageInfo.LastWriteTime.UtcDateTime)
                {
                    // 判断 SHA256 太慢，默认不判断

                    // 不需要更新的内容
                    fileStorageInfoList.Add(fileStorageInfo);
                    continue;
                }
            }

            if (source.Length == 0)
            {
                // 没有长度的文件，那就只记录，不需要加密
                fileStorageInfoList.Add(new FileStorageInfo(source.Name, relativePath,
                    new DateTimeOffset(source.LastWriteTimeUtc), source.Length, string.Empty,
                    string.Empty, Array.Empty<int>()));

                continue;
            }

            var fileByteList = await File.ReadAllBytesAsync(source.FullName);

            var sha256 = SHA256.HashData(fileByteList);
            var hash = ByteListToString(sha256);

            // 如果存在重复的 HASH 文件，那就不需要重复加密
            var (success, salt, path) = TryFindPath(fileStorageInfoList, hash);
            if (!success)
            {
                // 如果没有找到，那就从之前存放的列表找一下，例如文件被移动等
                (success, salt, path) = TryFindPath(currentFileStorageInfoDictionary.Values, hash);
            }

            if (!success)
            {
                // 加上盐，这样就不会存在大量文件实现相同的密码加密，统计方式可以更加降低
                salt = CreateSalt();
                var key = Key.Concat(salt).ToArray();

                var encryptionFileByteList =
                    BinaryEncryption.Encrypt(fileByteList, key, suffixData: SuffixData, random: Random.Shared);

                path = Path.Join(packageDirectory.FullName, hash);
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);

                await File.WriteAllBytesAsync(path, encryptionFileByteList);
            }

            if (Path.IsPathFullyQualified(path))
            {
                // 如果路径包含绝对路径，就需要转换为相对路径，方便后续解密使用
                path = Path.GetRelativePath(TargetDirectory.FullName, path);
            }

            fileStorageInfoList.Add(new FileStorageInfo(source.Name, relativePath,
                new DateTimeOffset(source.LastWriteTimeUtc), source.Length, hash, path,
                salt));
        }

        await SaveIndexFile(fileStorageInfoList);
    }

    private static (bool success, int[] salt, string path) TryFindPath(IEnumerable<FileStorageInfo> fileStorageInfoList,
        string sha256)
    {
        var fileStorageInfo = fileStorageInfoList.FirstOrDefault(t => string.Equals(t.SHA256, sha256, StringComparison.Ordinal));

        if (fileStorageInfo is not null)
        {
            return (true, fileStorageInfo.Salt, fileStorageInfo.Path);
        }

        return (false, null!, null!);
    }

    private async Task<EncryptionDirectoryIndexFileInfo> DecryptIndexFile(string indexFile)
    {
        var indexFileByteList = await File.ReadAllBytesAsync(indexFile);
        var indexByteList = BinaryEncryption.Decrypt(indexFileByteList, Key, suffixData: SuffixData);
        if (indexByteList is null)
        {
            throw new KeyErrorEncryptionDirectoryException();
        }

        var encryptionDirectoryIndexFileInfo =
            JsonSerializer.Deserialize<EncryptionDirectoryIndexFileInfo>(indexByteList.AsSpan());

        if (encryptionDirectoryIndexFileInfo is null || !string.Equals(encryptionDirectoryIndexFileInfo.HeaderText,
                EncryptionDirectoryIndexFileInfo.DefaultHeaderText, StringComparison.Ordinal))
        {
            throw new KeyErrorEncryptionDirectoryException();
        }

        return encryptionDirectoryIndexFileInfo;
    }

    private static int[] CreateSalt()
    {
        var count = Random.Shared.Next(3, 6);
        var salt = new int[count];
        for (int i = 0; i < count; i++)
        {
            salt[i] = Random.Shared.Next();
        }

        return salt;
    }

    private static string ByteListToString(byte[] byteList) => string.Join("", byteList.Select(t => t.ToString("X2")));

    private async Task SaveIndexFile(List<FileStorageInfo> fileStorageInfoList)
    {
        var indexFile = Path.Join(TargetDirectory.FullName, EncryptionDirectoryIndexFileName);

        if (File.Exists(indexFile))
        {
            // 先判断是否有旧的，如果有的话，那就备份一下吧
            var maxVersion = 0;

            foreach (var file in TargetDirectory.GetFiles("*", SearchOption.TopDirectoryOnly))
            {
                var match = Regex.Match(file.Name, EncryptionDirectoryIndexVersionRegex);
                if (match.Success)
                {
                    if (int.TryParse(match.Groups[1].Value, out var version))
                    {
                        maxVersion = Math.Max(maxVersion, version);
                    }
                }
            }

            maxVersion++;

            File.Move(indexFile,
                Path.Join(TargetDirectory.FullName,
                    string.Format(CultureInfo.InvariantCulture, EncryptionDirectoryIndexVersionName, maxVersion)));
        }

        Directory.CreateDirectory(Path.GetDirectoryName(indexFile)!);

        var encryptionDirectoryIndexFileInfo =
            new EncryptionDirectoryIndexFileInfo(EncryptionDirectoryIndexFileInfo.DefaultHeaderText,
                fileStorageInfoList);
        var fileByteList = JsonSerializer.SerializeToUtf8Bytes(encryptionDirectoryIndexFileInfo);
        var encryptFileByteList =
            BinaryEncryption.Encrypt(fileByteList, Key, suffixData: SuffixData, random: Random.Shared);
        await File.WriteAllBytesAsync(indexFile, encryptFileByteList);
    }

    /// <summary>
    /// 后缀
    /// </summary>
    ///  string.Join(',', System.Linq.Enumerable.Repeat(0, 32).Select(t => "0x" + ((byte) Random.Shared.Next()).ToString("X2")))
    private byte[] SuffixData { get; } = new byte[]
    {
        0x5E, 0xF9, 0xF4, 0xA2, 0x15, 0xA3, 0x5E, 0x74, 0xF6, 0x63, 0x5E, 0x36, 0x31, 0x6B, 0x2A, 0x72, 0x3E, 0x14,
        0x26, 0x69, 0x69, 0x23, 0x66, 0x3D, 0xA4, 0xE5, 0x9B, 0xD9, 0x26, 0xA4, 0x9B, 0xF7
    };

    private const string EncryptionDirectoryIndexFileName = "Index.data";
    private const string EncryptionDirectoryIndexVersionName = "Index_{0}.data";
    private const string EncryptionDirectoryIndexVersionRegex = /*lang=regex*/ @"Index_(\d*)\.data";
}
