using Amazon.S3;
using Amazon.S3.Model;
using dkapi.Data;
using Microsoft.EntityFrameworkCore;

namespace dkapi;

public class ProfileEnpoints
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/upload-profile", async (IFormFile file, string userId, IS3Service s3, DkdbContext db) =>
        {
            var user = await db.DkUsers.Where(e => e.Id == userId).FirstOrDefaultAsync();
            if (user == null)
                return Results.BadRequest("user not found");
            var key = await s3.PutProfileImage(file);
            user.ProfileKey = key;
            await db.SaveChangesAsync();
            return Results.Ok("profile uploaded");
        }).DisableAntiforgery();

        app.MapGet("/me", async (DkdbContext db, IAmazonS3 s3, string userId) =>
                {
                    var user = await db.DkUsers
                        .Where(e => e.Id == userId)
                        .Select(e => new
                        {
                            e.UserName,
                            e.Email,
                            e.PhoneNumber,
                            e.ProfileKey,
                            e.Address
                        })
                        .FirstOrDefaultAsync();
                    if (user == null)
                        return null;

                    var awsConfig = app.Configuration.GetSection("AWS");
                    var url = string.Empty;
                    if (user?.ProfileKey != string.Empty)
                    {
                        var preSignUrlRequest = new GetPreSignedUrlRequest
                        {
                            Key = awsConfig.GetValue<string>("Folder") + "/" + user?.ProfileKey,
                            Expires = DateTime.UtcNow.AddHours(1),
                            BucketName = awsConfig.GetValue<string>("BucketName")
                        };
                        url = await s3.GetPreSignedURLAsync(preSignUrlRequest);
                    }
                    var profile = new ProfileDTO
                    {
                        UserName = user?.UserName,
                        Email = user?.Email,
                        PhoneNumber = user?.PhoneNumber,
                        ProfileUrl = url,
                        Address = user?.Address
                    };
                    return profile;
                });

    }
}
