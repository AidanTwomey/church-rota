using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ChurchRota.Library.Data;
using ChurchRota.Api.Model;

namespace ChurchRota.Api.Queries;

public class PhoneNumberSanitiser
{
    public string Sanitize(string phoneNumber)
    {
        // Remove any non-numeric characters
        return new string(phoneNumber.Where(char.IsDigit).ToArray());
    }
}

public class GetReader
{
    private readonly PhoneNumberSanitiser sanitiser;

    public GetReader(PhoneNumberSanitiser sanitiser)
    {
        this.sanitiser = sanitiser;
    }

    public async Task<IResult> Handle(ChurchRotaContext db, string id)
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
