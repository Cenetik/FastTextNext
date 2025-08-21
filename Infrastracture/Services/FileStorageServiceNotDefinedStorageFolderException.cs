
namespace Infrastracture.Services
{
    [Serializable]
    internal class FileStorageWithFolderCheckServiceNotDefinedStorageFolderException : Exception
    {
        public FileStorageWithFolderCheckServiceNotDefinedStorageFolderException()
        {
        }

        public FileStorageWithFolderCheckServiceNotDefinedStorageFolderException(string? message) : base(message)
        {
        }

        public FileStorageWithFolderCheckServiceNotDefinedStorageFolderException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}