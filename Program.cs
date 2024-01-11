using Amazon.S3;
using dkapi;
using dkapi.Data;
using dkapi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

var rootDir = Directory.GetCurrentDirectory();
var dotenv = Path.Combine(rootDir, ".env");
DotEnvParser.Load(dotenv);

builder.Configuration.AddEnvironmentVariables();

var DB_USER = Environment.GetEnvironmentVariable("DB_USER");
var DB_PASS = Environment.GetEnvironmentVariable("DB_PASS");
var DB_HOST = Environment.GetEnvironmentVariable("DB_HOST");
var DB_PORT = Environment.GetEnvironmentVariable("DB_PORT");
var DB_NAME = Environment.GetEnvironmentVariable("DB_NAME");

builder.Environment.EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

var connectionString = $"Host={DB_HOST}; Database={DB_NAME}; User Id={DB_USER}; Port={DB_PORT}; Password={DB_PASS}; Include Error Detail=true";

builder.Services.AddDbContext<DkdbContext>(options =>
{
    options.UseNpgsql(connectionString);
});


builder.Services.AddIdentityApiEndpoints<DkUser>()
    .AddEntityFrameworkStores<DkdbContext>();
builder.Services.AddAuthorizationBuilder();


// configure aws s3 bucket
var awsConfig = builder.Configuration.GetSection("AWS");
var serviceUrl = awsConfig.GetValue<string>("ServiceURL");
var s3config = new AmazonS3Config
{
    RegionEndpoint = Amazon.RegionEndpoint.USEast1,
    ForcePathStyle = true,
};
s3config.ServiceURL = serviceUrl;
builder.Services.AddSingleton<IAmazonS3>(svp => new AmazonS3Client(
    awsConfig.GetValue<string>("AwsAccessKey"),
    awsConfig.GetValue<string>("AwsSecretKey"),
    s3config
));


builder.Services.AddSingleton<IS3Service, S3Service>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetService<DkdbContext>();
    db.Database.Migrate();
    if (!db.Categories.Any() && !db.ShippingStatuses.Any() && !db.Products.Any() && !db.Discounts.Any())
    {
        SeedData.SeedCategories(app);
        SeedData.SeedShippingStatus(app);
        SeedData.SeedDiscount(app);
        SeedData.SeedProduct(app);
    }
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.MapIdentityApi<DkUser>();
app.UseAuthorization();

// ComputerEnpoints.Map(app);
ProductEnpoints.Map(app);
OrderEnpoint.Map(app);




app.Run();