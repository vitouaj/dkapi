using dkapi;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// add dbcontext
// builder.Services.AddDbContext<DkdbContext>(
//     options => options.UseSqlite($"Data Source={DbPath}"));
var DB_CONNECTION = Environment.GetEnvironmentVariable("DB_CONNECTION");


// var connectionString = builder.Configuration.GetConnectionString("Postgres");

builder.Services.AddDbContext<DkdbContext>(options =>
{
    options.UseNpgsql(DB_CONNECTION);
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
        var dbctservice = scope.ServiceProvider.GetService<DkdbContext>();
        ApplyMigrations(dbctservice);
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


void ApplyMigrations(DkdbContext context)
{
    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }
}
