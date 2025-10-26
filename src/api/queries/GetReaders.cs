using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ChurchRota.Library.Data;
using Microsoft.AspNetCore.Mvc;
using ChurchRota.Api.Model;

namespace ChurchRota.Api.Queries;

public static class GetReaders
{
    public static async Task<IResult> Handle(ChurchRotaContext db)
    {
        // Find people who have an associated PeopleRole pointing to the Role "Reader"
        var readers = await db.People
            .Include(p => p.PeopleRoles!)
                .ThenInclude(pr => pr.Role)
            .Where(p => p.PeopleRoles!.Any(pr => pr.Role != null && pr.Role.RoleName == "Reader"))
            .ToListAsync();

        var response = new List<Reader>();
        foreach (var r in readers)
        {
            response.Add(new Reader { Name = $"{r.FirstName} {r.LastName}" });
        }
        return Results.Ok(response);
    }
}

internal class CreatedTextResult : IResult
{
    private readonly string _content;
    private readonly string _contentType;
    private readonly string _location;

    public CreatedTextResult(string content, string contentType, string location)
    {
        _content = content;
        _contentType = contentType;
        _location = location;
    }

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = StatusCodes.Status201Created;
        if (!string.IsNullOrEmpty(_location))
        {
            httpContext.Response.Headers["Location"] = _location;
        }
        httpContext.Response.ContentType = _contentType;
        await httpContext.Response.WriteAsync(_content);
    }
}
