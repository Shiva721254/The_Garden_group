# The Garden Group — Incident Management System (NoSQL Project)

**Repository:** The_Garden_group  
**Tech:** ASP.NET Core MVC (C#) + MongoDB Atlas + Cookie Authentication + Bootstrap

This application allows employees to create incident tickets and track their status. Service Desk employees can manage tickets for all employees and manage employee accounts. The project is implemented **strictly according to the NoSQL rubric** (two roles only: `employee` and `serviceDesk`).

---

## Rubric Compliance (Strict Mode)

- **Rights management:** `employee` vs `serviceDesk`
- **Employee must:** login, create ticket, view own tickets, dashboard with % open / % resolved / % closed
- **ServiceDesk must:** login, dashboard for all tickets, CRUD tickets for all employees (incl. status updates), CRUD employees
- **Individual feature:** implemented as a separate class (not part of client requirements)

---

## Features

### Employee (`employee`)

- Dashboard (own ticket statistics + percentages)
- Create Ticket
- My Tickets (only own tickets)

### Service Desk (`serviceDesk`)

- Dashboard (global ticket statistics for all employees)
- Tickets management: list all tickets, edit status + resolution note, delete ticket
- Employee management (CRUD): Create / Edit / Delete employees at `/Employees`

---

## Individual Functionality (Separate Class)

**TicketSearchService** (`Services/TicketSearchService.cs`)

- Searches in ticket **Subject + Description**
- Supports **AND / OR** search modes
- Sorted **newest first** (`CreatedAt desc`)

---

## Setup

### Prerequisites

- .NET SDK (see `.csproj`)
- MongoDB Atlas
- Visual Studio Community (recommended)

### Environment (.env)

Create a `.env` file in the project root:

```env
MONGODB_URI=mongodb+srv://<user>:<password>@<cluster>/


Seeded Demo Accounts (Strict Mode)

ServiceDesk

Email: servicedesk@company.com

Password: P@ssw0rd!

Employee

Email: employee@company.com

Password: P@ssw0rd!
```



Main Routes
Auth

GET /Auth/Login

POST /Auth/Login

POST /Auth/Logout

GET /Auth/Denied




Employee (role: employee)

GET /Employee/Dashboard

GET /Tickets/Create

POST /Tickets/Create

GET /Tickets/MyTickets

ServiceDesk (role: serviceDesk)

GET /ServiceDesk/Dashboard

GET /Tickets (all tickets + search AND/OR)

GET /Tickets/Edit/{id}

POST /Tickets/Edit

POST /Tickets/Delete/{id}

GET /Employees

GET /Employees/Create

POST /Employees/Create

GET /Employees/Edit/{id}

POST /Employees/Edit/{id}

GET /Employees/Delete/{id}

POST /Employees/Delete/{id}



Project Structure (High Level)

Controllers/ — MVC controllers

Models/ — MongoDB entities (Employee, Ticket)

ViewModels/ — form/view models

Repositories/ — MongoDB data access

Services/ — business logic (Auth, Dashboard, Seed, Search)

Views/ — Razor views

wwwroot/ — static assets



Authors / Contributions

Shiva Lamichhane ( Group 5)

Implemented authentication + role-based authorization (employee vs serviceDesk)

MongoDB Atlas integration + repository layer

Employee features (dashboard + create ticket + my tickets)

ServiceDesk features (global dashboard + ticket management + employee CRUD)

Individual feature: TicketSearchService (AND/OR + newest-first)

UI/UX with Bootstrap + validation
