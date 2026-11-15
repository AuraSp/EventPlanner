# EventPlanner

> **One idea, five stages, 48 versions.**  
> A C# learning project that grows from a small console program into a database-backed Web API.

EventPlanner helps keep track of events such as weddings and other celebrations. Each event has a name, date, time, location, number of guests and notes. The idea was inspired by a real event-planning business, so the features are based on tasks an organizer might actually need.

The purpose of this repository is not just to show the final program. It also shows **how and why the program changed** as new programming concepts were introduced.

---

## Project at a glance

| | |
|---|---|
| **Language** | C# |
| **Application types** | Console application → ASP.NET Core Web API |
| **Data storage** | In-memory collections → JSON → SQLite |
| **Database explored** | Dapper and Entity Framework Core |
| **Security** | User accounts, password hashing and bearer-token authentication |
| **API documentation** | Swagger / OpenAPI |
| **Versions** | `milestone1` through `milestone48` |

### The main idea

An organizer needs to know **what is happening, where, when and for whom**. In the first version, that information is stored in a simple list. Later, it stays after restarts, belongs to individual users, moves into a relational database and becomes available through HTTP requests.

```text
Console menu                 Web API
     │                          │
     ▼                          ▼
 User input                HTTP request
     │                          │
     └──────────┬───────────────┘
                ▼
          Application logic
                │
                ▼
           Data storage
        (memory / JSON / SQL)
```

---

## 01 · The learning journey

The assignment has **five main stages**. I kept additional numbered folders as working snapshots while developing each stage. This makes the changes easier to inspect without losing older versions.

| Assignment stage | Saved versions | Main change | Why it matters |
|---|---|---|---|
| **01 · Basic console application** | `milestone1`–`milestone15` | Event menu, input, CRUD operations and JSON | Learn how a program accepts, processes and saves information. |
| **02 · Object-oriented design** | `milestone16`–`milestone22` | `Event` class, constructors and validation | Give related data and rules a clear home. |
| **03 · Layers and users** | `milestone23`–`milestone39` | Services, repository interfaces, accounts and event ownership | Separate responsibilities and prepare for different storage systems. |
| **04 · Relational database** | `milestone40`–`milestone47` | SQLite, Dapper and EF Core | Store connected data reliably and learn two ways to work with SQL. |
| **05 · Web API** | `milestone48` | HTTP endpoints, DTOs, authentication and Swagger | Make EventPlanner accessible to a client through standard web requests. |

**Important:** The numbered folders are **saved development versions**, while the five stages above are the assignment milestones. Each folder preserves a particular point in the project's evolution; it is not a separate unrelated application.

### Stage 01 — From an idea to a working console program

**Versions:** `milestone1`–`milestone15`

I started with a text menu that allows the user to add, list, find and delete events. The early versions focus on basic C# building blocks: variables, conditions, loops, methods and collections.

As the program grew, I added input validation, filtering and sorting. JSON storage was introduced so the event list would not disappear whenever the application closed.

**What this stage taught me:** A program needs more than a working menu. It must also handle invalid input, keep its data organized and decide what should happen when something goes wrong.

### Stage 02 — Giving events their own model

**Versions:** `milestone16`–`milestone22`

In the beginning, event information was stored in a flexible collection of strings. This worked for learning the basics, but it did not clearly describe what a valid event should look like.

I introduced an `Event` class with properties, a constructor and validation. Event objects could then be stored in a `List<Event>` and serialized to JSON.

**What changed:** Instead of passing loosely connected values around the program, I could work with one meaningful object.

**What this stage taught me:** Object-oriented programming is useful when a group of values and the rules around them belong together.

### Stage 03 — Separating responsibilities and introducing users

**Versions:** `milestone23`–`milestone39`

The program became harder to maintain when the menu, business rules and file operations were mixed together. I separated them into distinct responsibilities:

- **Console menu:** shows choices and collects user input.
- **Services:** apply application rules, including event ownership.
- **Repositories:** define how events and users are read and saved.
- **Models:** represent events and users.

This stage also introduced user registration and login. Passwords are not stored as plain text: the password service uses salted SHA hashes. Events are connected to an owner using `UserId`, so each user works with their own event list.

**What this stage taught me:** Layers make a project easier to change. For example, the user interface should not need to know whether events come from a JSON file or a database.

### Stage 04 — Moving from files to a relational database

**Versions:** `milestone40`–`milestone47`

JSON is convenient for a small project, but a relational database is more suitable for structured, connected information. I added **SQLite** with `Users` and `Events` tables and a relationship between them.

I first used **Dapper**, which maps SQL query results to C# objects while keeping the SQL statements visible. Later, I explored **Entity Framework Core**, including a database context and migrations.

The project retains earlier storage approaches so their differences can be compared.

**What this stage taught me:** Database design is connected to application design. The `UserId` relationship is not just another field: it links event ownership in the program to the stored data.

### Stage 05 — Turning the application into a Web API

**Version:** `milestone48`

The final stage changes the entry point from a console menu to an **ASP.NET Core Web API**. Instead of typing menu options, a client sends HTTP requests. The API validates the request, applies the existing rules and returns a response with an appropriate HTTP status code.

This stage includes:

- REST-style event endpoints using **GET, POST, PUT and DELETE**.
- **DTOs** to separate API input/output from database models.
- Input validation and HTTP responses such as **200, 201, 204, 400, 401, 404** and **409**.
- Registration and login endpoints.
- ASP.NET Core **bearer-token authentication** for protected event routes.
- **Swagger / OpenAPI** for exploring and testing the endpoints in a browser.
- EF Core and SQLite for persistent storage.

Each event request uses the authenticated user's ID. In testing, one account could see its own events, while another account could not read or delete an event belonging to the first account.

**What this stage taught me:** A Web API allows different programs to communicate over the internet using HTTP. I also learned why checking user input, protecting user accounts, and returning clear responses are important.

---

## 02 · Project structure

```text
EventPlanner/
├── milestone1/       Early console version
├── milestone2/       Next saved version
├── ...
├── milestone15/      End of Stage 01
├── milestone16/      Start of Stage 02
├── ...
├── milestone22/      End of Stage 02
├── milestone23/      Start of Stage 03
├── ...
├── milestone39/      End of Stage 03
├── milestone40/      Start of Stage 04
├── ...
├── milestone47/      End of Stage 04
├── milestone48/      Stage 05: ASP.NET Core Web API
└── README.md         Project guide
```

---

## 03 · How to run the project

### Requirements

- A compatible **.NET SDK** installed (the later versions were developed with .NET 10 tooling).
- A terminal or an IDE such as JetBrains Rider / Visual Studio.
- No external database server: the SQL versions use **SQLite**.

First, check that the .NET command is available:

```bash
dotnet --version
```

If your environment uses a custom SDK location, replace `dotnet` in the examples with the full path to your .NET executable.

### Run any saved console version (Stages 01–04)

Choose the folder you want to inspect. For example:

```bash
cd milestone22
dotnet restore
dotnet run
```

To inspect another stage, return to the repository root and run a different folder:

```bash
cd ../milestone39
dotnet restore
dotnet run
```

The console interface and available storage options differ by version, because the features were added gradually.

### Run the final Web API (Stage 05)

From the repository root:

```bash
cd milestone48
dotnet restore
dotnet run
```

The API listens at:

- **API:** `http://127.0.0.1:5080/`
- **Swagger UI:** `http://127.0.0.1:5080/swagger`

When the API starts, it automatically updates the database if needed. The project uses a local SQLite database, so there is no need to install a separate database server.

**Note:** The API must keep running while you test it in the browser. Closing the application stops the server, but does not automatically delete data stored in SQLite.

---

## 04 · API reference — `milestone48`

### Authentication

| Method | Endpoint | Purpose | Typical success |
|---|---|---|---|
| `POST` | `/api/auth/register` | Create an account | `201 Created` |
| `POST` | `/api/auth/login` | Check credentials and issue a token | `200 OK` |

Registration can return `400 Bad Request` for invalid input or `409 Conflict` for an existing username. Login returns `401 Unauthorized` when the credentials are not accepted.

The project uses ASP.NET Core's built-in bearer-token authentication. These tokens are **not JWTs**. Clients send the access token using an `Authorization: Bearer ...` header.

### Events — authentication required

| Method | Endpoint | Purpose | Typical success |
|---|---|---|---|
| `GET` | `/api/events` | List the current user's events | `200 OK` |
| `GET` | `/api/events/{id}` | Find one of the current user's events | `200 OK` |
| `POST` | `/api/events` | Create an event | `201 Created` |
| `PUT` | `/api/events/{id}` | Update an owned event | `200 OK` |
| `DELETE` | `/api/events/{id}` | Delete an owned event | `204 No Content` |

An invalid request may return `400`; a missing event or an event owned by another user returns `404`; and a missing or invalid token returns `401`.

### Example event request

```json
{
  "type": "Wedding",
  "name": "Summer celebration",
  "date": "2027-06-12",
  "time": "15:30",
  "location": "Vilnius",
  "guestCount": 80,
  "notes": "Outdoor ceremony"
}
```

The API accepts `Wedding` or `Other` as the event type. Names and locations cannot be empty, the guest count must be positive, and dates/times must use the required formats. New events must have a date within the permitted planning period.

### Test with Swagger

1. Start `milestone48` and open `http://127.0.0.1:5080/swagger`.
2. Use **POST `/api/auth/register`** if you need a test account.
3. Use **POST `/api/auth/login`** with that account.
4. After a successful login, Swagger is configured to authorize its requests with the returned access token automatically.
5. Try **GET `/api/events`** or create an event with **POST `/api/events`**.

---

## 05 · How the backend and database connect

The final design follows a simple path:

```text
HTTP client / Swagger
        │
ASP.NET Core endpoint --> Request DTO + validation
        │
Authenticated user identity
        │
EventService / application rules
        │
Repository / EF Core DbContext
        │
SQLite database: Users -- < Events
        │
HTTP response DTO + status code
```

Each layer solves a different problem:

| Part | Responsibility                                                |
|---|---------------------------------------------------------------|
| **Endpoint** | Understand the HTTP request and choose an HTTP response       |
| **DTO** | Describe the information client is allowed to send or receive |
| **Model** | Represent an event or user inside the application             |
| **Service** | Apply rules such as event ownership                           |
| **Repository / DbContext** | Read or change persistent data                                |
| **SQLite** | Store users and events between application runs               |

The Web API uses EF Core to update events and the EventService to handle other event operations. This is a learning project, so not every operation is built in exactly the same way.

---

## 06 · Validation, security and testing

### Input validation

The project checks that event information is correct before saving it. It makes sure required fields are filled in, the event type is valid, the date and time are written correctly, and the number of guests is greater than zero. If something is wrong, the system shows an error instead of saving the event.

### Account security

- Passwords are stored as **salted SHA256 hashes**, not readable passwords.
- Login provides an access token; protected endpoints require it.
- Events have an owner (`UserId`), and API queries are restricted to the authenticated account.
- Requests for another user's event return `404`, rather than exposing that event's details.

### Checks performed during development

| Test | Result |
|---|---|
| Register and log in | Passed |
| Create an event through the API | Passed |
| Read an event list | Passed |
| Update an event | Passed |
| Delete an event | Passed |
| Use a valid bearer token | Passed |
| View one account's events separately from another's | Passed |
| Try to read another user's event by ID | Returned `404` |
| Try to delete another user's event by ID | Returned `404`; original event remained |

During debugging, some manually copied tokens caused `401 Unauthorized`. Directly passing the newly issued token worked. The final Swagger test setup can authorize requests automatically after login, avoiding manual token copying.

---

## 07 · What changed — and why

This project follows a recurring pattern:

**A simple solution works - its visible limitations - new concept solves a specific problem.**

| Earlier approach | Limitation | Later improvement |
|---|---|---|
| Data stored only in memory | Lost after closing the program | JSON persistence |
| Loosely grouped event fields | Hard to validate and maintain | `Event` model |
| Menu directly responsible for everything | Difficult to change or test | Services and repositories |
| All events treated as one shared list | No individual ownership | Users, authentication and `UserId` |
| Files as the main source of stored data | Limited relational structure | SQLite database |
| Handwritten SQL access | More repetitive mapping code | EF Core as an additional approach |
| Local console interaction | Only usable through the console | ASP.NET Core HTTP API |

This is the main lesson of EventPlanner: **architecture is introduced to solve problems, not simply to add more files or classes.**

---