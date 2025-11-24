using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace BackEnd.Infrastructure.Services
{
    public interface IEmailSenderService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody);
        Task<bool> SendSurveyInvitationEmailAsync(string toEmail, string userName, string surveyTitle, string surveyDescription, string verificationLink);
    }

    public class EmailSenderService : IEmailSenderService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailSenderService> _logger;

        public EmailSenderService(IConfiguration configuration, ILogger<EmailSenderService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            try
            {
                var fromEmail = _configuration["Gmail:FromEmail"];
                var appPassword = _configuration["Gmail:AppPassword"];

                if (string.IsNullOrWhiteSpace(fromEmail) || string.IsNullOrWhiteSpace(appPassword))
                {
                    _logger.LogError("Gmail configuration is missing");
                    return false;
                }

                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(fromEmail));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = subject;
                email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = htmlBody
                };

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(fromEmail, appPassword);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation($"Email sent successfully to {toEmail}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {toEmail}");
                return false;
            }
        }

        public async Task<bool> SendSurveyInvitationEmailAsync(
            string toEmail, 
            string userName, 
            string surveyTitle, 
            string surveyDescription, 
            string verificationLink)
        {
            var subject = $"Zaproszenie do wypełnienia ankiety: {surveyTitle}";
            
            var htmlBody = GenerateSurveyInvitationEmailTemplate(
                userName, 
                surveyTitle, 
                surveyDescription, 
                verificationLink);

            return await SendEmailAsync(toEmail, subject, htmlBody);
        }

        private string GenerateSurveyInvitationEmailTemplate(
            string userName,
            string surveyTitle,
            string surveyDescription,
            string verificationLink)
        {
            return $@"
                <html>
                <head>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            background-color: #f4f4f4;
                            margin: 0;
                            padding: 0;
                        }}
                        .container {{
                            max-width: 600px;
                            margin: 50px auto;
                            background-color: #ffffff;
                            padding: 20px;
                            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                            border-radius: 10px;
                        }}
                        .header {{
                            text-align: center;
                            padding: 10px 0;
                            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                            border-radius: 10px 10px 0 0;
                            margin: -20px -20px 20px -20px;
                        }}
                        .header h1 {{
                            color: #ffffff;
                            margin: 20px 0;
                        }}
                        .content {{
                            line-height: 1.6;
                            padding: 20px 0;
                            color: #333333;
                        }}
                        .content p {{
                            margin: 10px 0;
                        }}
                        .survey-info {{
                            background-color: #f8f9fa;
                            padding: 15px;
                            border-left: 4px solid #667eea;
                            margin: 20px 0;
                            border-radius: 4px;
                        }}
                        .survey-info h3 {{
                            margin-top: 0;
                            color: #667eea;
                        }}
                        .button {{
                            display: block;
                            width: 250px;
                            margin: 30px auto;
                            padding: 15px;
                            text-align: center;
                            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                            color: white !important;
                            text-decoration: none;
                            border-radius: 25px;
                            font-weight: bold;
                            font-size: 16px;
                            box-shadow: 0 4px 15px rgba(102, 126, 234, 0.4);
                        }}
                        .button:hover {{
                            box-shadow: 0 6px 20px rgba(102, 126, 234, 0.6);
                        }}
                        .link-fallback {{
                            word-break: break-all;
                            color: #667eea;
                            font-size: 12px;
                        }}
                        .footer {{
                            text-align: center;
                            padding: 20px 0;
                            font-size: 12px;
                            color: #777777;
                            border-top: 1px solid #eeeeee;
                            margin-top: 20px;
                        }}
                        .icon {{
                            font-size: 48px;
                            margin: 10px 0;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <div class='icon'>📋</div>
                            <h1>Zaproszenie do Ankiety</h1>
                        </div>
                        <div class='content'>
                            <p>Witaj <strong>{userName}</strong>!</p>
                            <p>Zostałeś zaproszony do wypełnienia ankiety. Twoja opinia jest dla nas bardzo ważna!</p>
                            
                            <div class='survey-info'>
                                <h3>📊 {surveyTitle}</h3>
                                <p>{surveyDescription ?? "Prosimy o wypełnienie poniższej ankiety."}</p>
                            </div>

                            <p>Kliknij w poniższy przycisk, aby rozpocząć:</p>
                            <a href='{verificationLink}' class='button'>Rozpocznij Ankietę</a>
                            
                            <p style='text-align: center; color: #999; font-size: 14px;'>
                                Link jest ważny przez 1 godzinę
                            </p>

                            <p style='margin-top: 30px;'>Jeśli przycisk nie działa, skopiuj i wklej poniższy adres URL do przeglądarki:</p>
                            <p class='link-fallback'>{verificationLink}</p>
                        </div>
                        <div class='footer'>
                            <p>© 2024 Survey System. Wszelkie prawa zastrzeżone.</p>
                            <p>Jeśli nie prosiłeś o tę wiadomość, zignoruj ten email.</p>
                        </div>
                    </div>
                </body>
                </html>";
        }
    }
}
