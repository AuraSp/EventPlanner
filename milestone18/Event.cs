class Event
{
    public int Id { get; set; }
    public string Type { get; set; }
    public string Name { get; set; }
    public string Date { get; set; }
    public string Time { get; set; }
    public string Location { get; set; }
    public int GuestCount { get; set; }
    public string Notes { get; set; }
    
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