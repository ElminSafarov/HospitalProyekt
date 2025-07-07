using System.Collections.Generic;

public class Department
{
    public string Name { get; set; }
    public List<Doctor> Doctors { get; set; }

    public Department(string name)
    {
        Name = name;
        Doctors = new List<Doctor>();
    }
}