using Amazon.S3;

namespace dkapi;

public interface IS3Service
{
    Task<Stream> GetSingleImage(string bucketName, string key);
    Task<string> PutSingleImage(IFormFile file);
    Task<List<string>> PutImagesToBucket(IFormFileCollection files);
}
