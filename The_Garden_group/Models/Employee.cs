using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace The_Garden_Group.Models;


public sealed class Employee
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";

    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";

    // "employee" or "serviceDesk"
    public string Role { get; set; } = "employee";

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}