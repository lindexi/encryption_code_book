using System;

namespace EncryptionDirectory;

public class EncryptionDirectoryException : Exception
{
    public EncryptionDirectoryException()
    {
    }

    public EncryptionDirectoryException(string? message) : base(message)
    {
    }

    public EncryptionDirectoryException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
