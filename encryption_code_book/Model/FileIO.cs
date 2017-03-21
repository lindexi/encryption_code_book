using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Provider;
using UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding;

namespace encryption_code_book.Model
{
    public static class FileIO
    {
        public static async Task<string> ReadTextAsync(StorageFile file)
        {
            CachedFileManager.DeferUpdates(file);

            string str = await Windows.Storage.FileIO.ReadTextAsync(file);
         
            FileUpdateStatus updateStatus = await CachedFileManager.CompleteUpdatesAsync(file);
            return str;
        }

        public static async Task WriteTextAsync(StorageFile file, string contents)
        {
            await WriteTextAsync(file, contents, UnicodeEncoding.Utf8);
        } 

        public static async Task WriteTextAsync(StorageFile file, string contents, UnicodeEncoding encoding )
        {
            await Windows.Storage.FileIO.WriteTextAsync(file, contents,encoding);


        }
    }
}
