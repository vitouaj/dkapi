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

#pragma warning disable CS8601 // Possible null reference assignment.
builder.Environment.EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
#pragma warning restore CS8601 // Possible null reference assignment.

var connectionString = $"Host={DB_HOST}; Database={DB_NAME}; User Id={DB_USER}; Port={DB_PORT}; Password={DB_PASS}";

builder.Services.AddDbContext<DkdbContext>(options =>
{
    options.UseNpgsql(connectionString);
});



builder.Services.AddIdentityApiEndpoints<DkUser>()
    .AddEntityFrameworkStores<DkdbContext>();
builder.Services.AddAuthorizationBuilder();


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
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetService<DkdbContext>();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        db.Database.Migrate();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    }
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapIdentityApi<DkUser>();
app.UseAuthorization();


app.MapGet("/homepage", () =>
{
    return "this is homepage";
})
.RequireAuthorization()
.WithOpenApi();


// get all computers
app.MapGet("/computer", async (DkdbContext db) =>
{
    return await db.Computers.ToArrayAsync();
})
.WithOpenApi();

// post a computer
app.MapPost("/computer", async (DkdbContext db, Computer pc) =>
{
    if (pc == null)
        return "pc is null";

    await db.Computers.AddAsync(pc);
    await db.SaveChangesAsync();
    return $"pc id: {pc.Id}";
})
.RequireAuthorization()
.WithOpenApi();

// get computer by Id
app.MapGet("/computer/{computerId}", async (string computerId, DkdbContext db) =>
{
    if (computerId == null)
    {
        return null;
    }
    var pc = await db.Computers.SingleOrDefaultAsync(c => c.Id == new Guid(computerId));
    return pc;
})
.RequireAuthorization()
.WithOpenApi();


app.Run();