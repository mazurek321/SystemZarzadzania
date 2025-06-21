using Infrastructure.Email.EmailTypes;

namespace Infrastructure.Email;
public class EmailTemplateFactory
{
    private readonly Dictionary<EmailType, IEmailTemplate> _templates;

    public EmailTemplateFactory(IEnumerable<IEmailTemplate> templates)
    {
        _templates = templates.ToDictionary(t => t.Type, t => t);
    }

    public IEmailTemplate GetTemplate(EmailType type)
    {
        if (_templates.TryGetValue(type, out var template))
            return template;

        throw new NotSupportedException($"No template registered for type {type}");
    }
}