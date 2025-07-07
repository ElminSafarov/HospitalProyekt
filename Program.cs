using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Net;
using System.Net.Mail;

class Program
{
    static List<Department> departments = new List<Department>();

    static void Main(string[] args)
    {
        InitializeDepartments();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("\uD83D\uDCCB X\u0259st\u0259xana Q\u0259bul Sistemine Xo\u015F G\u0259ldiniz!");
            Console.WriteLine("1. Yeni istifade\u00E7i il\u0259 rezervasiya");
            Console.WriteLine("2. B\u00FCt\u00FCn rezervasiyalara bax (Admin)");
            Console.WriteLine("ESC - \u00C7\u0131x\u0131\u015F");

            var key = Console.ReadKey();

            if (key.Key == ConsoleKey.D1 || key.Key == ConsoleKey.NumPad1)
            {
                Console.Clear();
                Console.Write("Ad\u0131n\u0131z: ");
                string firstName = Console.ReadLine();

                Console.Write("Soyad\u0131n\u0131z: ");
                string lastName = Console.ReadLine();

                Console.Write("Email: ");
                string email = Console.ReadLine();

                Console.Write("Telefon: ");
                string phone = Console.ReadLine();

                User user = new User(firstName, lastName, email, phone);

                Console.WriteLine("\n\uD83D\uDCCD Z\u0259hm\u0259t olmasa \u015F\u0259b\u0259 secin:");
                for (int i = 0; i < departments.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {departments[i].Name}");
                }

                int deptChoice = GetValidChoice(1, departments.Count) - 1;
                Department selectedDepartment = departments[deptChoice];

                Console.WriteLine($"\n\uD83D\uDC68\u200D⚕️ {selectedDepartment.Name} \u015F\u0259b\u0259sind\u0259 \u00E7al\u0131\u015Fan h\u0259kiml\u0259r:");
                for (int i = 0; i < selectedDepartment.Doctors.Count; i++)
                {
                    var doc = selectedDepartment.Doctors[i];
                    Console.WriteLine($"{i + 1}. {doc.GetFullName()} ({doc.ExperienceYears} il t\u0259cr\u00FCb\u0259)");
                }

                int doctorChoice = GetValidChoice(1, selectedDepartment.Doctors.Count) - 1;
                Doctor selectedDoctor = selectedDepartment.Doctors[doctorChoice];

                SelectAppointmentSlot(user, selectedDoctor);
            }
            else if (key.Key == ConsoleKey.D2 || key.Key == ConsoleKey.NumPad2)
            {
                Console.Clear();
                if (AdminLogin())
                {
                    Console.Clear();
                    FileService.ShowAllAppointments();
                }
                Console.WriteLine("\nDavam etm\u0259k \u00FC\u00E7\u00FCn ENTER...");
                Console.ReadLine();
            }
            else if (key.Key == ConsoleKey.Escape)
            {
                break;
            }
        }
    }

    static void InitializeDepartments()
    {
        var pediatriya = new Department("Pediatriya");
        pediatriya.Doctors.Add(new Doctor("Aytac", "Quliyeva", 5));
        pediatriya.Doctors.Add(new Doctor("V\u00FCsal", "\u018Fliyev", 7));
        pediatriya.Doctors.Add(new Doctor("Nigar", "\u018Fs\u0259dova", 4));

        var travmatologiya = new Department("Travmatologiya");
        travmatologiya.Doctors.Add(new Doctor("H\u00FCseyn", "M\u0259mm\u0259dov", 10));
        travmatologiya.Doctors.Add(new Doctor("\u018Ali", "M\u0259mm\u0259dli", 6));

        var stomatologiya = new Department("Stomatologiya");
        stomatologiya.Doctors.Add(new Doctor("Leyla", "Ta\u011F\u0131yeva", 8));
        stomatologiya.Doctors.Add(new Doctor("Murad", "S\u0259f\u0259rov", 3));
        stomatologiya.Doctors.Add(new Doctor("Z\u0259hra", "\u018Fhm\u0259dli", 5));
        stomatologiya.Doctors.Add(new Doctor("Orxan", "\u0130smay\u0131lov", 9));

        departments.Add(pediatriya);
        departments.Add(travmatologiya);
        departments.Add(stomatologiya);
    }

    static void SelectAppointmentSlot(User user, Doctor doctor)
    {
        Console.WriteLine("\n\uD83D\uDCC5 Z\u0259hm\u0259t olmasa g\u00F6r\u00FC\u015F tarixi daxil edin (format: yyyy-MM-dd):");
        string date;
        while (true)
        {
            date = Console.ReadLine();
            if (DateTime.TryParse(date, out DateTime parsedDate)) break;
            Console.WriteLine("❌ D\u00FCzg\u00FCn tarix daxil edin (n\u00FCmun\u0259: 2025-07-10):");
        }

        while (true)
        {
            var slots = doctor.GetSlotsForDate(date);

            Console.WriteLine($"\n⏰ {doctor.GetFullName()} \u00FC\u00E7\u00FCn {date} tarixind\u0259 m\u00F6vcud saatlar:");
            for (int i = 0; i < slots.Count; i++)
            {
                var slot = slots[i];
                string status = slot.IsReserved ? "❌ Rezerv olunub" : "✅ M\u00F6vcuddur";
                Console.WriteLine($"{i + 1}. {slot.TimeRange} - {status}");
            }

            int slotChoice = GetValidChoice(1, slots.Count) - 1;
            var selectedSlot = slots[slotChoice];

            if (selectedSlot.IsReserved)
            {
                Console.WriteLine("\n⚠️ Bu saat art\u0131q rezerv olunub. Ba\u015Fqa bir saat secin.");
                continue;
            }

            selectedSlot.IsReserved = true;

            Console.WriteLine($"\n✅ T\u0259\u015F\u0259kk\u00FCrl\u0259r, {user.FirstName} {user.LastName}!");
            Console.WriteLine($"Siz {date} tarixind\u0259 saat {selectedSlot.TimeRange} \u00FC\u00E7\u00FCn {doctor.GetFullName()} h\u0259kimin q\u0259buluna yaz\u0131ld\u0131n\u0131z.");

            // Gmail göndər
            MailService.SendConfirmationEmail(user.Email, $"{user.FirstName} {user.LastName}", doctor.GetFullName(), date, selectedSlot.TimeRange);

            // JSON-a qeyd et
            var record = new AppointmentRecord
            {
                UserFullName = $"{user.FirstName} {user.LastName}",
                UserEmail = user.Email,
                DoctorFullName = doctor.GetFullName(),
                DepartmentName = departments.First(d => d.Doctors.Contains(doctor)).Name,
                Date = date,
                TimeRange = selectedSlot.TimeRange
            };
            FileService.SaveAppointment(record);

            break;
        }
    }

    static int GetValidChoice(int min, int max)
    {
        int choice;
        while (!int.TryParse(Console.ReadLine(), out choice) || choice < min || choice > max)
        {
            Console.WriteLine("Z\u0259hm\u0259t olmasa d\u00FCzg\u00FCn secim edin:");
        }
        return choice;
    }

    static bool AdminLogin()
    {
        Console.Write("\n\uD83D\uDD11 Admin \u015Fifr\u0259sini daxil edin: ");
        string password = Console.ReadLine();
        if (password == "aker123")
        {
            Console.WriteLine("✅ Giri\u015F u\u011Furlu!");
            return true;
        }
        Console.WriteLine("❌ \u015Eifr\u0259 yaln\u0131\u015Fd\u0131r! Giri\u015F r\u0259dd edildi.");
        return false;
    }
}