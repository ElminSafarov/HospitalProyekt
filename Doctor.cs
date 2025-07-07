using System.Collections.Generic;

public class Doctor
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int ExperienceYears { get; set; }

    public Dictionary<string, List<AppointmentSlot>> Schedule { get; set; }

    public Doctor(string firstName, string lastName, int experienceYears)
    {
        FirstName = firstName;
        LastName = lastName;
        ExperienceYears = experienceYears;
        Schedule = new Dictionary<string, List<AppointmentSlot>>();
    }

    public List<AppointmentSlot> GetSlotsForDate(string date)
    {
        if (!Schedule.ContainsKey(date))
        {
            Schedule[date] = new List<AppointmentSlot>
            {
                new AppointmentSlot("09:00-11:00"),
                new AppointmentSlot("12:00-14:00"),
                new AppointmentSlot("15:00-17:00")
            };
        }
        return Schedule[date];
    }

    public string GetFullName()
    {
        return $"{FirstName} {LastName}";
    }
}