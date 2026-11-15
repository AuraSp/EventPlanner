using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

//register Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Configure Bearer authentication for Swagger
    options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Description = "Paste only the accessToken from /api/auth/login. Do not include 'Bearer', quotation marks, or spaces."
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("bearer", document)] = []
        });
});

//register database and event services.
builder.Services.AddScoped<EventPlannerDbContext>(_ =>
    new EventPlannerDbContext(
        EventPlannerDbContextFactory.CreateOptions()
    )
);

builder.Services.AddScoped<IEventRepository, EfEventRepository>();
builder.Services.AddScoped<EventService>();

//user authentication services
builder.Services.AddScoped<IUserRepository, EfUserRepository>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<AuthService>();

//enable bearer-token authentication.
builder.Services
    .AddAuthentication(BearerTokenDefaults.AuthenticationScheme)
    .AddBearerToken();

builder.Services.AddAuthorization();

//build application
var app = builder.Build();

//apply any pending database migrations when the API starts.
using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider
        .GetRequiredService<EventPlannerDbContext>();

    database.Database.Migrate();
}

//enable Swagger
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    // Automatically authorize Swagger after a successful login.
    options.UseResponseInterceptor("""
                                   (response) => {
                                       if (response.url?.includes("/api/auth/login") &&
                                           Number(response.status) === 200) {

                                           try {
                                               const data = response.data &&
                                                            typeof response.data === "object"
                                                   ? response.data
                                                   : JSON.parse(response.text);

                                               if (data?.accessToken) {
                                                   window.ui.preauthorizeApiKey(
                                                       "bearer",
                                                       data.accessToken
                                                   );
                                               }
                                           } catch (error) {
                                               console.warn("Automatic authorization failed.");
                                           }
                                       }

                                       return response;
                                   }
                                   """);
});

app.UseAuthentication();
app.UseAuthorization();

//first API endpoint
app.MapGet("/", () =>
{
    return Results.Ok(new
    {
        message = "EventPlanner API is running!"
    });
});

//REGISTER NEW USER!!
app.MapPost("/api/auth/register",
        (RegisterRequest request, AuthService authService) =>
        {
            try
            {
                User user = authService.Register(
                    request.Username,
                    request.Password
                );

                return Results.Json(
                    new
                    {
                        id = user.Id,
                        username = user.Username
                    },
                    statusCode: StatusCodes.Status201Created
                );
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(new
                {
                    message = ex.Message
                });
            }
        })
    .WithTags("Authentication");

//LOGIN
app.MapPost("/api/auth/login",
        (LoginRequest request, AuthService authService) =>
        {
            User? user = authService.Login(
                request.Username,
                request.Password
            );

            if (user == null)
            {
                return Results.Unauthorized();
            }

            // Store the authenticated user's identity in the token.
            Claim[] claims =
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var identity = new ClaimsIdentity(
                claims,
                BearerTokenDefaults.AuthenticationScheme
            );

            var principal = new ClaimsPrincipal(identity);

            return Results.SignIn(
                principal,
                authenticationScheme: BearerTokenDefaults.AuthenticationScheme
            );
        })
    .WithTags("Authentication");


//GET ALL EVENTS BELONGING TO THE LOGGED-IN USER
app.MapGet("/api/events",
        (ClaimsPrincipal user, EventService eventService) =>
        {
            string? id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(id, out int userId))
            {
                return Results.Unauthorized();
            }

            List<EventResponse> events = eventService
                .GetForUser(userId)
                .Select(EventResponse.FromEvent)
                .ToList();

            return Results.Ok(events);
        })
    .RequireAuthorization()
    .WithTags("Events")
    .Produces<List<EventResponse>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized);



//GET ONE EVENT BY ID
app.MapGet("/api/events/{id:int}",
        (int id, ClaimsPrincipal user, EventService eventService) =>
        {
            string? currentId =
                user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(currentId, out int userId))
            {
                return Results.Unauthorized();
            }

            Event? eventData = eventService.FindForUser(userId, id);

            if (eventData == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(EventResponse.FromEvent(eventData));
        })
    .RequireAuthorization()
    .WithTags("Events")
    .Produces<EventResponse>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized)
    .Produces(StatusCodes.Status404NotFound);



//CREATE EVENT
app.MapPost("/api/events",
        (SaveEventRequest request,
            ClaimsPrincipal user,
            EventService eventService) =>
        {
            string? currentId =
                user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(currentId, out int userId))
                return Results.Unauthorized();

            string? error = EventRequestValidator.Validate(request);

            if (error != null)
                return Results.BadRequest(new { message = error });

            try
            {
                Event newEvent = eventService.AddForUser(
                    userId,
                    request.Type!,
                    request.Name!.Trim(),
                    request.Date!,
                    request.Time!,
                    request.Location!.Trim(),
                    request.GuestCount,
                    request.Notes ?? ""
                );

                return Results.Created(
                    $"/api/events/{newEvent.Id}",
                    EventResponse.FromEvent(newEvent)
                );
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        })
    .RequireAuthorization()
    .WithTags("Events")
    .Produces<EventResponse>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status401Unauthorized);


//UPDATE EVENT
app.MapPut("/api/events/{id:int}",
        async (int id,
            SaveEventRequest request,
            ClaimsPrincipal user,
            EventPlannerDbContext database) =>
        {
            string? currentId =
                user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(currentId, out int userId))
                return Results.Unauthorized();

            string? error = EventRequestValidator.Validate(request);

            if (error != null)
                return Results.BadRequest(new { message = error });

            Event? eventData = await database.Events
                .FirstOrDefaultAsync(e =>
                    e.Id == id && e.UserId == userId);

            if (eventData == null)
                return Results.NotFound();

            try
            {
                eventData.UpdateDetails(
                    request.Type!,
                    request.Name!.Trim(),
                    request.Date!,
                    request.Time!,
                    request.Location!.Trim(),
                    request.GuestCount,
                    request.Notes ?? ""
                );

                await database.SaveChangesAsync();

                return Results.Ok(EventResponse.FromEvent(eventData));
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        })
    .RequireAuthorization()
    .WithTags("Events")
    .Produces<EventResponse>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status401Unauthorized)
    .Produces(StatusCodes.Status404NotFound);



//DELETE EVENT
app.MapDelete("/api/events/{id:int}",
        (int id,
            ClaimsPrincipal user,
            EventService eventService) =>
        {
            string? currentId =
                user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(currentId, out int userId))
                return Results.Unauthorized();

            bool deleted = eventService.DeleteForUser(userId, id);

            if (!deleted)
                return Results.NotFound();

            return Results.NoContent();
        })
    .RequireAuthorization()
    .WithTags("Events")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status401Unauthorized)
    .Produces(StatusCodes.Status404NotFound);




//start the web server
app.Run("http://127.0.0.1:5080");