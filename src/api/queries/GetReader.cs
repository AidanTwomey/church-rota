using Microsoft.EntityFrameworkCore;
using ChurchRota.Library.Data;
using ChurchRota.Api.Model;
using ChurchRota.Api.Utilities;

namespace ChurchRota.Api.Queries;

public class GetReader
{
    private readonly PhoneNumberSanitiser sanitiser;
    private readonly ChurchRotaContext db;

    public GetReader(ChurchRotaContext db, PhoneNumberSanitiser sanitiser)
    {
        this.db = db;
        this.sanitiser = sanitiser;
    }

    public async Task<IResult> Handle(string id)
    {
        // Sanitize incoming id and find the person by phone first
        var sanitized = sanitiser.Sanitize(id);

        var person = await db.People
            .Include(p => p.PeopleRoles!)
                .ThenInclude(pr => pr.Role)
            .SingleOrDefaultAsync(p => p.Phone == sanitized);

        if (person == null)
        {
            return Results.NotFound();
        }

        // Confirm they have the Reader role
        var hasReader = person.PeopleRoles != null && person.PeopleRoles.Any(pr => pr.Role != null && pr.Role.RoleName == "Reader");

        return hasReader
            ? Results.Ok(new Reader { Name = $"{person.FirstName} {person.LastName}" })
            : Results.NotFound();
    }
}
