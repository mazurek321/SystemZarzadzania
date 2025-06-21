using Infrastructure.Email;

namespace Infrastructure.Email.EmailTypes;
public class Registration : IEmailTemplate
{
    public EmailType Type => EmailType.Registration;

    public string GetSubject() => "Registration cofirm!";

    public string GetHtmlBody(string message) => $@"
        <html>
        <body style='font-family: sans-serif;'>
            <h1 style='color: green;'>Registered successfully.</h1>
            <p>Your account has been created successfully. Thank you for joining us!</p>
            <p style='color: grey;'>Email: {message}</p>
            <div style='display: flex; align-items: center; gap: 25px;'>
                <h4>SystemZarzadzania</h4>
                <p>Project</p>
            </div>
        </body>
        </html>";
}
