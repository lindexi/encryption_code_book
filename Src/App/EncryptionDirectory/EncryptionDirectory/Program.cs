using System.IO;
using System.Threading.Tasks;

namespace EncryptionDirectory;

class Program
{
    static async Task Main(string[] args)
    {
        var directoryEncryption =
            new DirectoryEncryption(new int[] { '林', '德', '熙' }, new DirectoryInfo(@"."),
                new DirectoryInfo(@"文件夹"));
        await directoryEncryption.UpdateAsync();

        await directoryEncryption.DecryptDirectoryAsync(new DirectoryInfo(@"1"));
    }
}
