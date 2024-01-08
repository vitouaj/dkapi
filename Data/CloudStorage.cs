using Google.Cloud.Storage.V1;
using System;
using System.IO;

namespace dkapi;
public class CloudStorage
{
    public void UploadFile(
        string bucketName = "your-unique-bucket-name",
        string localPath = "my-local-path/my-file-name",
        string objectName = "my-file-name")
    {
        var storage = StorageClient.Create();
        using var fileStream = File.OpenRead(localPath);
        storage.UploadObject(bucketName, objectName, null, fileStream);
        Console.WriteLine($"Uploaded {objectName}.");
    }
    public void DownloadFile(
        string bucketName = "your-unique-bucket-name",
        string objectName = "my-file-name",
        string localPath = "my-local-path/my-file-name")
    {
        var storage = StorageClient.Create();
        using var outputFile = File.OpenWrite(localPath);
        storage.DownloadObject(bucketName, objectName, outputFile);
        Console.WriteLine($"Downloaded {objectName} to {localPath}.");
    }
}