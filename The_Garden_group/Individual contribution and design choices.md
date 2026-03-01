The Garden Group — Individual Contribution & Design Choices

Project: The_Garden_Group (ASP.NET Core MVC + MongoDB Atlas)
Student: Shiva Lamichhane (Group 5)
Student Number: 721254
Date: 28 feb 2026

1) What is submitted

	This submission includes all code required to run the application:

	Controllers/, Models/, ViewModels/, Repositories/, Services/, Views/, wwwroot/

	Program.cs, appsettings.json, .csproj

	SeedService for generating demo data (employees + tickets)

	MongoDB connection is configured via .env using MONGODB_URI (credentials are not committed for security).


2) My individual contribution (components developed)

	This work represents my individual part of the original group project. I implemented the following components:

	Authentication & Authorization

	Cookie-based login/logout

	Role-based access control with two roles: employee and serviceDesk

	Role-based navigation and access denied handling
	Files: AuthController, AuthService, _Layout, Program.cs

	Tickets (Employee + ServiceDesk)

	Employee: create ticket + view own tickets

	ServiceDesk: view all tickets, edit ticket (including status changes + resolution note), delete ticket
	Files: TicketsController, TicketRepository, ticket views

	Employees (ServiceDesk-only CRUD)

	ServiceDesk can list/create/edit/delete employees at /Employees

	Roles restricted to employee or serviceDesk
	Files: EmployeesController, EmployeeRepository, EmployeeFormVm, employee views

	Dashboards

	Employee dashboard: own ticket statistics + percentages

	ServiceDesk dashboard: global ticket statistics + percentages
	Files: DashboardService, DashboardVm, EmployeeController, ServiceDeskController

	Individual functionality (separate class)

	Advanced ticket search with AND/OR mode, searches subject + description, newest-first sorting
	Files: TicketSearchService, tickets list UI/controller integration


3) Design and implementation choices (reasoning)

	Database design: two MongoDB collections employees and tickets. Tickets reference employee using CreatedByUserId (1 employee → many tickets). Referencing was chosen to keep tickets queryable globally for ServiceDesk and to avoid unbounded embedded arrays.

	Architecture: MVC + Services + Repositories to separate UI, business logic, and MongoDB access, reducing duplication and improving maintainability.

	Security: passwords stored as BCrypt hashes; POST forms protected with anti-forgery tokens; cookie auth for server-rendered MVC flow.

	Strict rubric mode: only two roles (employee, serviceDesk) to match the required rights management model.


4) How to run / verify

	Add .env with MONGODB_URI=...

	Run (F5) → /Auth/Login

	Demo accounts:

	ServiceDesk: servicedesk@company.com / P@ssw0rd!

	Employee: employee@company.com / P@ssw0rd!

	Verify:

	Employee: dashboard + create ticket + my tickets

	ServiceDesk: tickets CRUD + employees CRUD + dashboards

	Search: /Tickets with AND/OR mode and newest-first results