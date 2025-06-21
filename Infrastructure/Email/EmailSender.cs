using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Infrastructure.Email.EmailTypes;

namespace Infrastructure.Email;

public class EmailSender
{
    private readonly EmailInfo _emailSettings;
    private readonly EmailTemplateFactory _templateFactory;

    public EmailSender(IOptions<EmailInfo> emailSettings, EmailTemplateFactory templateFactory)
    {
        _emailSettings = emailSettings.Value;
        _templateFactory = templateFactory;
    }

    public async Task SendEmailAsync(string recipientEmail, string message, EmailType type)
    {
        var template = _templateFactory.GetTemplate(type);

        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
        email.To.Add(MailboxAddress.Parse(recipientEmail));
        email.Subject = template.GetSubject();

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = template.GetHtmlBody(message)
        };

        email.Body = bodyBuilder.ToMessageBody();

        using var smtp = new SmtpClient();

        smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;

        await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_emailSettings.SenderUsername, _emailSettings.SenderPassword);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}
