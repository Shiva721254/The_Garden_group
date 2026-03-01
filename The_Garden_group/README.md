# The Garden Group — Incident Management System (NoSQL)

ASP.NET Core MVC + MongoDB (Atlas/Compass) incident/ticket system with **two roles only**:
- `employee`
- `serviceDesk`

---

## Run
1. Open solution in Visual Studio
2. Run (F5)
3. App: `https://localhost:7157` → `/Auth/Login`

---

## Database & Seed
- DB: `garden_group_db`
- Collections: `employees`, `tickets`
- On startup `SeedService` creates demo data:
  - 50 employees (employee + serviceDesk)
  - 120 tickets (mixed statuses)
  - Total: 170 documents (meets 100+ requirement)

Re-seed:
- Clear `employees` and `tickets` collections
- Restart app

---

## Demo Accounts
Login: `/Auth/Login`

**Employee**
- `employee@company.com` / `P@ssw0rd!`
- Access: `/Employee/Dashboard`, `/Tickets/Create`, `/Tickets/MyTickets`

**ServiceDesk**
- `servicedesk@company.com` / `P@ssw0rd!`
- Access: `/ServiceDesk/Dashboard`, `/Tickets` (CRUD + search), `/Employees` (CRUD)

---

## Features

### Employee (`employee`)
- Dashboard with counts + **% Open / % Resolved / % Closed** (own tickets)
- Create ticket
- View own tickets

### ServiceDesk (`serviceDesk`)
- Dashboard with counts + percentages (**all tickets**)
- Tickets: list all, edit (status + resolution note), delete
- Employees: create/edit/delete employees + assign role (employee/serviceDesk)

---

## Individual Feature (Separate Class)
**Advanced Ticket Search** — `Services/TicketSearchService.cs`
- Searches `Subject` + `Description`
- Supports **AND / OR** mode
- Sorted by newest first (`CreatedAt desc`)
- Used from `/Tickets?query=...&mode=AND|OR`

---

## Structure (Where to find things)
- `Controllers/` — Auth, Employee, ServiceDesk, Tickets, Employees
- `Models/` — `Employee`, `Ticket`
- `ViewModels/` — `LoginVm`, `TicketCreateVm`, `TicketEditVm`, `EmployeeFormVm`, `DashboardVm`
- `Repositories/` — MongoDB CRUD layer
- `Services/` — Auth, Dashboard, Seed, TicketSearch
- `Views/` — Razor pages
- `wwwroot/` — static assets

---

## Docs
- `DATABASE_ERD.md` — ERD + collection structure
- `PROJECT_DOCUMENTATION.md` — architecture + decisions
- `95_POINTS_CHECKLIST.md` — grading proof checklist