class Event
{
    public int Id { get; private set; }
    public string Type { get; private set; }
    public string Name { get; private set; }
    public string Date { get; private set; }
    public string Time { get; private set; }
    public string Location { get; private set; }
    public int GuestCount { get; private set; }
    public string Notes { get; private set; }
    
    public Event(
        int id,
        string type,
        string name,
        string date,
        string time,
        string location,
        int guestCount,
        string notes)
    {
        
        Id = id;
        Type = type;
        Name = name;
        Date = date;
        Time = time;
        Location = location;
        GuestCount = guestCount;
        Notes = notes;
    }
}