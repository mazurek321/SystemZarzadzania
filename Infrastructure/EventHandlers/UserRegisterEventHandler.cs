using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Email;
using Infrastructure.Email.EmailTypes;
using Core.Events;
using Newtonsoft.Json;


namespace Infrastructure.EventHandlers;
public class UserRegisteredEventHandler : INotificationHandler<UserRegisteredEvent>
{
    private readonly EmailSender _emailSender;

    public UserRegisteredEventHandler(EmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        var user = notification.User;

        if (string.IsNullOrWhiteSpace(user.Email))
        {
            throw new InvalidOperationException("User email is missing.");
        }

        await _emailSender.SendEmailAsync(user.Email, user.Email, EmailType.Registration);
        Console.WriteLine("Email sent to: " + user.Email);
    }
}
