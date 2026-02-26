namespace The_Garden_Group.Constants;

/// <summary>
/// Application-wide constants for user roles.
/// </summary>
public static class Roles
{
    public const string Employee = "employee";
    public const string ServiceDesk = "serviceDesk";
    
    public static readonly string[] AllRoles = { Employee, ServiceDesk };
}

/// <summary>
/// Application-wide constants for ticket statuses.
/// </summary>
public static class TicketStatuses
{
    public const string Open = "open";
    public const string Resolved = "resolved";
    public const string Closed = "closed";
    
    public static readonly string[] AllStatuses = { Open, Resolved, Closed };
}

/// <summary>
/// Application-wide constants for ticket priorities.
/// </summary>
public static class TicketPriorities
{
    public const string Low = "low";
    public const string Medium = "medium";
    public const string High = "high";
    
    public static readonly string[] AllPriorities = { Low, Medium, High };
}
