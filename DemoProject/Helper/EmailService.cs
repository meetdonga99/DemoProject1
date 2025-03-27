using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;

namespace DemoProject.Helper
{
	public static class EmailService
	{
        public static bool Send(string toEmail, string subject, string body)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587, // Use 465 for SSL, 587 for TLS
                    Credentials = new NetworkCredential("meetdonga9@gmail.com", "hpqa ftrl vaxc cvop"),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("meetdonga9@gmail.com", "Meet Donga"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true // Set true if sending HTML content
                };

                mailMessage.To.Add(toEmail);

                smtpClient.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }
    }
}