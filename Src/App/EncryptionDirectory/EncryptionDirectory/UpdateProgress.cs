namespace EncryptionDirectory;

public record UpdateProgress(string CurrentFileName, int CurrentFileIndex, string RelativePath, int FileCount)
{
}
