using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public static class FileService
{
    private static string filePath = "appointments.json";

    public static void SaveAppointment(AppointmentRecord record)
    {
        List<AppointmentRecord> appointments = new List<AppointmentRecord>();

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            if (!string.IsNullOrWhiteSpace(json))
                appointments = JsonSerializer.Deserialize<List<AppointmentRecord>>(json);
        }

        appointments.Add(record);

        string updatedJson = JsonSerializer.Serialize(appointments, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, updatedJson);
    }

    public static void ShowAllAppointments()
    {
        Console.WriteLine("📋 Bütün rezervasiyalar:\n");

        if (!File.Exists(filePath))
        {
            Console.WriteLine("Hələlik heç bir rezervasiya yoxdur.");
            return;
        }

        string json = File.ReadAllText(filePath);
        if (string.IsNullOrWhiteSpace(json))
        {
            Console.WriteLine("Hələlik heç bir rezervasiya yoxdur.");
            return;
        }

        var appointments = JsonSerializer.Deserialize<List<AppointmentRecord>>(json);
        int count = 1;
        foreach (var app in appointments)
        {
            Console.WriteLine($"{count++}. 👤 {app.UserFullName} | 📧 {app.UserEmail}");
            Console.WriteLine($"   🏥 {app.DepartmentName} | 👨‍⚕️ {app.DoctorFullName}");
            Console.WriteLine($"   📅 {app.Date} | ⏰ {app.TimeRange}\n");
        }
    }
}