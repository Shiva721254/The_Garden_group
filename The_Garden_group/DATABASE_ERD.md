# Database Entity-Relationship Diagram (ERD)
## The Garden Group - Incident Management System

---

## 📊 **Database Overview**

**Database Type:** MongoDB (NoSQL Document Database)  
**Database Name:** `garden_group_db`  
**Collections:** 2 (employees, tickets)  
**Total Documents:** 170+ (50 employees + 120 tickets)

---

## 🗂️ **Collection Schemas**

### **1. employees Collection**

Stores all user accounts including regular employees and service desk personnel.

#### **Schema Structure:**

```json
{
  "_id": ObjectId,              // Auto-generated MongoDB unique identifier
  "FirstName": String,          // Employee's first name (required)
  "LastName": String,           // Employee's last name (required)
  "Email": String,              // Unique email address (required, indexed)
  "Role": String,               // "employee" or "serviceDesk" (required)
  "IsActive": Boolean,          // Account activation status (required)
  "PasswordHash": String,       // BCrypt hashed password (required)
  "CreatedAt": DateTime         // Account creation timestamp (required)
}
```

#### **Field Specifications:**

| Field | Type | Required | Unique | Default | Description |
|-------|------|----------|--------|---------|-------------|
| `_id` | ObjectId | Yes | Yes | Auto | MongoDB primary key |
| `FirstName` | String | Yes | No | - | Employee's given name |
| `LastName` | String | Yes | No | - | Employee's family name |
| `Email` | String | Yes | Yes | - | Login identifier (lowercase) |
| `Role` | String (enum) | Yes | No | - | "employee" or "serviceDesk" |
| `IsActive` | Boolean | Yes | No | true | Account enabled/disabled |
| `PasswordHash` | String | Yes | No | - | BCrypt hash (60 chars) |
| `CreatedAt` | DateTime | Yes | No | UtcNow | Account creation date |

#### **Indexes:**

- **Primary Index:** `_id` (automatic)
- **Secondary Index:** `Email` (unique, case-insensitive)
- **Filter Index:** `Role` (for role-based queries)

#### **Business Rules:**

1. Email must be unique across all employees
2. Email is stored in lowercase for consistency
3. Role must be either "employee" or "serviceDesk"
4. PasswordHash must be BCrypt encrypted (never store plain text)
5. IsActive = false for soft delete (maintain referential integrity)

#### **Sample Document:**

```json
{
  "_id": ObjectId("507f1f77bcf86cd799439011"),
  "FirstName": "John",
  "LastName": "Doe",
  "Email": "john.doe@company.com",
  "Role": "employee",
  "IsActive": true,
  "PasswordHash": "$2a$11$rK8eJ/K9h1Z5zW3w9J9z0uQ8w9J9z0uQ8w...",
  "CreatedAt": ISODate("2024-01-15T08:30:00.000Z")
}
```

---

### **2. tickets Collection**

Stores all incident/service desk tickets created by employees.

#### **Schema Structure:**

```json
{
  "_id": ObjectId,                  // Auto-generated MongoDB unique identifier
  "Subject": String,                // Ticket title/summary (required)
  "Description": String,            // Detailed problem description (required)
  "Status": String,                 // "open", "resolved", or "closed" (required)
  "Priority": String,               // "low", "medium", or "high" (required)
  "Category": String,               // Ticket category (required)
  "CreatedByUserId": ObjectId,      // Foreign key to employees._id (required)
  "AssignedToUserId": ObjectId,     // Foreign key to employees._id (optional)
  "CreatedAt": DateTime,            // Ticket creation timestamp (required)
  "UpdatedAt": DateTime,            // Last modification timestamp (required)
  "ResolutionNote": String,         // Solution details (optional)
  "ResolvedAt": DateTime            // Resolution timestamp (optional)
}
```

#### **Field Specifications:**

| Field | Type | Required | References | Default | Description |
|-------|------|----------|------------|---------|-------------|
| `_id` | ObjectId | Yes | - | Auto | MongoDB primary key |
| `Subject` | String | Yes | - | - | Brief title (max 200 chars) |
| `Description` | String | Yes | - | - | Detailed problem description |
| `Status` | String (enum) | Yes | - | "open" | Current ticket state |
| `Priority` | String (enum) | Yes | - | "medium" | Urgency level |
| `Category` | String | Yes | - | "other" | Problem classification |
| `CreatedByUserId` | ObjectId (FK) | Yes | employees._id | - | Ticket creator |
| `AssignedToUserId` | ObjectId (FK) | No | employees._id | null | Assigned service desk |
| `CreatedAt` | DateTime | Yes | - | UtcNow | Creation timestamp |
| `UpdatedAt` | DateTime | Yes | - | UtcNow | Last update timestamp |
| `ResolutionNote` | String | No | - | null | Solution explanation |
| `ResolvedAt` | DateTime | No | - | null | Resolution timestamp |

#### **Enumerated Values:**

**Status:**
- `open` - Newly created, awaiting service desk action
- `resolved` - Service desk has fixed the issue
- `closed` - Ticket is archived (with or without resolution)

**Priority:**
- `low` - Can be addressed within several days
- `medium` - Should be addressed within 1-2 days
- `high` - Requires immediate attention

**Category Examples:**
- Hardware
- Software
- Network
- Access
- Email
- Other

#### **Indexes:**

- **Primary Index:** `_id` (automatic)
- **Foreign Key Index:** `CreatedByUserId` (for user ticket queries)
- **Status Index:** `Status` (for filtering by status)
- **Timestamp Index:** `CreatedAt` (descending, for recent tickets)

#### **Business Rules:**

1. `CreatedByUserId` must reference a valid employee._id
2. `AssignedToUserId` (if set) must reference a valid serviceDesk employee
3. Status must be one of: "open", "resolved", "closed"
4. Priority must be one of: "low", "medium", "high"
5. `ResolutionNote` and `ResolvedAt` are only set when Status = "resolved"
6. `UpdatedAt` must be updated whenever any field changes
7. New tickets always start with Status = "open"

#### **Sample Document:**

```json
{
  "_id": ObjectId("507f1f77bcf86cd799439022"),
  "Subject": "Unable to login to email",
  "Description": "This issue started this morning and is blocking my work.",
  "Status": "resolved",
  "Priority": "high",
  "Category": "Email",
  "CreatedByUserId": ObjectId("507f1f77bcf86cd799439011"),
  "AssignedToUserId": ObjectId("507f1f77bcf86cd799439033"),
  "CreatedAt": ISODate("2024-03-10T09:15:00.000Z"),
  "UpdatedAt": ISODate("2024-03-10T14:30:00.000Z"),
  "ResolutionNote": "Reset password and configured email client",
  "ResolvedAt": ISODate("2024-03-10T14:30:00.000Z")
}
```

---

## 🔗 **Relationships**

### **employees ←→ tickets (One-to-Many)**

**Relationship Type:** One employee can create many tickets  
**Foreign Key:** `tickets.CreatedByUserId` → `employees._id`  
**Cardinality:** 1:N (one-to-many)  
**Referential Integrity:** Maintained by application layer

**Diagram:**
```
┌──────────────┐           ┌──────────────┐
│  employees   │           │   tickets    │
├──────────────┤           ├──────────────┤
│ _id (PK)     │───────┬───│ _id (PK)     │
│ FirstName    │       │   │ Subject      │
│ LastName     │       │   │ Description  │
│ Email        │       └──→│ CreatedByUserId (FK)
│ Role         │           │ Status       │
│ IsActive     │           │ Priority     │
│ PasswordHash │           │ Category     │
│ CreatedAt    │           │ CreatedAt    │
└──────────────┘           │ UpdatedAt    │
                           │ ResolutionNote│
                           │ ResolvedAt   │
                           └──────────────┘
```

### **employees (ServiceDesk) ←→ tickets (Optional Assignment)**

**Relationship Type:** One service desk employee can be assigned many tickets  
**Foreign Key:** `tickets.AssignedToUserId` → `employees._id` (nullable)  
**Cardinality:** 0..1:N (zero-or-one-to-many)  
**Constraint:** AssignedToUserId must reference an employee with Role="serviceDesk"

---

## 📈 **Data Volume & Performance**

### **Current Data:**

- **Employees:** 50 documents
  - 6 Service Desk users
  - 44 Regular employees
- **Tickets:** 120 documents
  - ~40 Open
  - ~40 Resolved
  - ~40 Closed

**Total:** 170 documents (exceeds 100+ requirement ✓)

### **Query Performance Optimizations:**

1. **Index on tickets.CreatedByUserId:** Fast lookup for "My Tickets" view
2. **Index on tickets.Status:** Quick filtering by status
3. **Index on tickets.CreatedAt (desc):** Efficient sorting by most recent
4. **Index on employees.Email (unique):** Fast login authentication
5. **Index on employees.Role:** Quick role-based access control

---

## 🔒 **Security & Validation**

### **Password Security:**
- Algorithm: BCrypt with default cost factor (10-12)
- Never store plain text passwords
- Passwords hashed on registration/password change

### **Data Validation:**
- Email format validated (regex)
- Required fields enforced in C# models
- Enum validation for Status, Priority, Role
- ObjectId format validation for foreign keys

### **Authorization:**
- Regular employees: Can only view/create their own tickets
- Service Desk: Full CRUD access to all tickets and employees
- Implemented via ASP.NET authorization policies

---

## 📝 **Notes**

1. **NoSQL Design Choice:** MongoDB chosen for flexible schema and horizontal scalability
2. **No Hard Deletes:** employees.IsActive used for soft delete to maintain ticket history
3. **Denormalization:** Employee names could be cached in tickets for performance (not implemented to maintain data integrity)
4. **Audit Trail:** CreatedAt, UpdatedAt, ResolvedAt provide complete ticket lifecycle tracking
5. **Search Optimization:** Text indexes could be added to Subject/Description for advanced search (current implementation uses regex)

---

**Last Updated:** March 2024  
**Database Version:** MongoDB 7.0+  
**Application:** The Garden Group Incident Management System
