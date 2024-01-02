using dkapi.Data;
using Microsoft.EntityFrameworkCore;

namespace dkapi;

public class ComputerEnpoints
{
    public static void Map(WebApplication app)
    {

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
    }
}
