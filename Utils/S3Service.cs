using System.CodeDom;
using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;

namespace dkapi;

public class S3Service : IS3Service
{
    private readonly IAmazonS3 client;
    private readonly IConfiguration configuration;
    public S3Service(IConfiguration _configuration, IAmazonS3 _client)
    {
        client = _client;
        configuration = _configuration;
    }

    public async Task<Stream> GetSingleImage(string key)
    {
        var bucketName = configuration.GetSection("AWS").GetValue<string>("BucketName");
        var getObjectResponse = await client.GetObjectAsync(bucketName, key);
        using var responseStream = getObjectResponse.ResponseStream;
        var stream = new MemoryStream();
        await responseStream.CopyToAsync(stream);
        stream.Position = 0;
        return stream;
    }


    public async Task<string> PutSingleImage(IFormFile file)
    {
        var awsConfig = configuration.GetSection("AWS");
        var objectName = DateTime.Now.ToString("yyyyMMddhhmmss") + file.FileName.ToLower();
        var bucketName = awsConfig.GetValue<string>("BucketName");
        bool bucketExist = await AmazonS3Util.DoesS3BucketExistV2Async(client, bucketName);
        if (!bucketExist)
        {
            var bucketRequest = new PutBucketRequest
            {
                BucketName = bucketName,
                UseClientRegion = true,
            };
            await client.PutBucketAsync(bucketRequest);
        }
        var request = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = objectName,
            InputStream = file.OpenReadStream(),
            ContentType = file.ContentType
        };
        var response = await client.PutObjectAsync(request);
        if (response.HttpStatusCode == HttpStatusCode.OK)
            return objectName;
        return string.Empty;
    }

    public async Task<List<string>> PutImagesToBucket(IFormFileCollection files)
    {
        var awsConfig = configuration.GetSection("AWS");
        List<string> result = [];
        foreach (var file in files)
        {
            var objectName = DateTime.Now.ToString("yyyyMMddhhmmss") + file.FileName.ToLower();
            var bucketName = awsConfig.GetValue<string>("BucketName");
            bool bucketExist = await AmazonS3Util.DoesS3BucketExistV2Async(client, bucketName);
            if (!bucketExist)
            {
                var bucketRequest = new PutBucketRequest
                {
                    BucketName = bucketName,
                    UseClientRegion = true,
                };
                await client.PutBucketAsync(bucketRequest);
            }
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = objectName,
                InputStream = file.OpenReadStream(),
                ContentType = file.ContentType
            };
            var response = await client.PutObjectAsync(request);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                result.Add(objectName);
        }
        return result;
    }
}
