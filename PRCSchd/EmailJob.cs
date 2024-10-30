using Quartz;
using System.Net;
using System.Net.Mail;


namespace PRCSchd
{
    public class EmailJob : IJob
    {
        private readonly IConfiguration _configuration;

        public EmailJob(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            string smtpServer = emailSettings["SmtpServer"];
            int smtpPort = int.Parse(emailSettings["SmtpPort"]);
            string senderEmail = emailSettings["SenderEmail"];
            string senderPassword = emailSettings["SenderPassword"];
            bool useSsl = bool.Parse(emailSettings["UseSsl"]);

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail),
                Subject = "Scheduled Email",
                Body = "This is a test email sent by the .NET scheduler",
                IsBodyHtml = true
            };
            //mailMessage.To.Add("recipient@example.com");

            //using var smtpClient = new SmtpClient(smtpServer, smtpPort)
            //{
            //    Credentials = new NetworkCredential(senderEmail, senderPassword),
            //    EnableSsl = useSsl
            //};
            mailMessage.To.Add("testuser@mailinator.com");

            using var smtpClient = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = useSsl
            };

            await smtpClient.SendMailAsync(mailMessage);
            Console.WriteLine("Email sent successfully!");
        }
    }
}
