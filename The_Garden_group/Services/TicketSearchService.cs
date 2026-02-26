using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Driver;
using The_Garden_Group.Models;

namespace The_Garden_Group.Services;

public sealed class TicketSearchService
{
    private readonly IMongoCollection<Ticket> _tickets;

    public TicketSearchService(IMongoDatabase db)
    {
        _tickets = db.GetCollection<Ticket>("tickets");
    }

    public async Task<List<Ticket>> SearchAsync(string? rawQuery, string? mode)
    {
        // If no query, return newest-first
        if (string.IsNullOrWhiteSpace(rawQuery))
        {
            return await _tickets.Find(_ => true)
                .SortByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        var terms = SplitTerms(rawQuery);

        if (terms.Count == 0)
        {
            return await _tickets.Find(_ => true)
                .SortByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        var f = Builders<Ticket>.Filter;

        FilterDefinition<Ticket> TermFilter(string term)
        {
            var safe = Regex.Escape(term);
            return f.Or(
                f.Regex(t => t.Subject, new BsonRegularExpression(safe, "i")),
                f.Regex(t => t.Description, new BsonRegularExpression(safe, "i"))
            );
        }

        var termFilters = terms.Select(TermFilter).ToList();
        var useAnd = string.Equals(mode ?? "OR", "AND", StringComparison.OrdinalIgnoreCase);

        var combined = useAnd ? f.And(termFilters) : f.Or(termFilters);

        return await _tickets.Find(combined)
            .SortByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    private static List<string> SplitTerms(string raw)
    {
        var parts = raw.Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

        return parts
            .Select(p => p.Trim())
            .Where(p => p.Length >= 2)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(10)
            .ToList();
    }
}