
using Infrastructure.Email.EmailTypes;
namespace Infrastructure.Email;

public interface IEmailTemplate
{
    EmailType Type { get; }
    string GetSubject();
    string GetHtmlBody(string message);
}
