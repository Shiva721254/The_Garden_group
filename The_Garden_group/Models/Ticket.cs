using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace The_Garden_Group.Models;


public sealed class Ticket
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Subject { get; set; } = "";
    public string Description { get; set; } = "";

    // open | resolved | closed
    public string Status { get; set; } = "open";

    // low | medium | high
    public string Priority { get; set; } = "medium";

    public string Category { get; set; } = "other";

    [BsonRepresentation(BsonType.ObjectId)]
    public string CreatedByUserId { get; set; } = "";

    [BsonRepresentation(BsonType.ObjectId)]
    public string? AssignedToUserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public string? ResolutionNote { get; set; }
    public DateTime? ResolvedAt { get; set; }
}