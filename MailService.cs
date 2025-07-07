//Gmail App Password Necə Yaradılır? (Bir dəfəlik addım)
//Gmail hesabına daxil ol
//Buradan keç: https://myaccount.google.com/security
//"2-Step Verification" aktiv et
//"App passwords" bölməsinə daxil ol
//Yeni şifrə yarat: App adı yaz (məsələn, HospitalApp), və sənə 16 rəqəmli xüsusi şifrə veriləcək


using System;
using System.Net;
using System.Net.Mail;

public static class MailService
{
    public static void SendConfirmationEmail(string toEmail, string userFullName, string doctorName, string date, string time)
    {
        try
        {
            string fromEmail = "Gmail"; // Əvəz et
            string appPassword = "Parol"; // Əvəz et (Gmail App Password)

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(fromEmail);
            mail.To.Add(toEmail);
            mail.Subject = "Qəbul Təsdiqi - Xəstəxana Sistemi";
            mail.Body = $"Əziz {userFullName},\n\nSiz {date} tarixində saat {time} üçün {doctorName} həkimin qəbuluna uğurla yazıldınız.\n\nTəşəkkür edirik!";

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential(fromEmail, appPassword);
            smtp.EnableSsl = true;

            smtp.Send(mail);
            Console.WriteLine("📧 Email göndərildi.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Email göndərilmədi: {ex.Message}");
        }
    }
}