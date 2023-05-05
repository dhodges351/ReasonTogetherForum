using Microsoft.WindowsAzure.Storage.Blob;

namespace ReasonTogetherForum.Data
{
    public interface IUpload
    {
        CloudBlobContainer GetBlobContainer(string connectionString, string containerName);
    }
}
