using dkapi;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var dir = Directory.GetCurrentDirectory();
var DbPath = Path.Join(dir, "dkapi.db");
Console.WriteLine($"DbPath: {DbPath}");

// add dbcontext
builder.Services.AddDbContext<DkdbContext>(
    options => options.UseSqlite($"Data Source={DbPath}"));

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
.RequireAuthorization()
.WithOpenApi();

// post a computer
app.MapPost("/computer", async (DkdbContext db, Computer pc) =>
{
    if (pc == null)
        return "pc is null";

    await db.Computers.AddAsync(pc);
    await db.SaveChangesAsync();
    return $"pc id: {pc.id}";
})
.RequireAuthorization()
.WithOpenApi();

// get computer by id
app.MapGet("/computer/{computerId}", async (string computerId, DkdbContext db) =>
{
    if (computerId == null)
    {
        return null;
    }
    var pc = await db.Computers.SingleOrDefaultAsync(c => c.id == new Guid(computerId));
    return pc;
})
.RequireAuthorization()
.WithOpenApi();


app.Run();
