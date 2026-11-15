using System.Globalization;

public record RegisterRequest(
    string Username,
    string Password
);

public record LoginRequest(
    string Username,
    string Password
);

public record EventResponse(
    int Id,
    string Type,
    string Name,
    string Date,
    string Time,
    string Location,
    int GuestCount,
    string Notes
)
{
    public static EventResponse FromEvent(Event eventData)
    {
        return new EventResponse(
            eventData.Id,
            eventData.Type,
            eventData.Name,
            eventData.Date,
            eventData.Time,
            eventData.Location,
            eventData.GuestCount,
            eventData.Notes
        );
    }
}


public record SaveEventRequest(
    string? Type,
    string? Name,
    string? Date,
    string? Time,
    string? Location,
    int GuestCount,
    string? Notes
);

public static class EventRequestValidator
{
    public static string? Validate(SaveEventRequest request)
    {
        if (request.Type is not ("Wedding" or "Other event"))
            return "Invalid event type.";

        if (string.IsNullOrWhiteSpace(request.Name))
            return "Event name cannot be empty.";

        if (string.IsNullOrWhiteSpace(request.Location))
            return "Location cannot be empty.";

        if (request.GuestCount <= 0)
            return "Guest count must be greater than zero.";

        if (!DateTime.TryParseExact(
                request.Date,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime date))
        {
            return "Invalid date. Use yyyy-MM-dd.";
        }

        if (date < DateTime.Today ||
            date > DateTime.Today.AddYears(3))
        {
            return "Event date must be within the next 3 years.";
        }

        if (!DateTime.TryParseExact(
                request.Time,
                "HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _))
        {
            return "Invalid time. Use HH:mm.";
        }

        return null;
    }
}