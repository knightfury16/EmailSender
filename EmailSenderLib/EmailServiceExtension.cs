using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using EmailSenderLib.SmtpEmailSender;

namespace EmailSenderLib;

public static class EmailServiceExtension
{
    public static IServiceCollection AddSmtpEmailSender(this IServiceCollection services, IConfigurationSection configurationSection)
    {
        services.Configure<SmtpEmailSettings>(configurationSection);
        services.AddSingleton<IEmailSender, SmtpEmailSender.SmtpEmailSender>();
        return services;
    }

    public static IServiceCollection AddSmtpEmailSender(this IServiceCollection services, Action<SmtpEmailSettings> configurationSettings)
    {
        services.Configure(configurationSettings);
        services.AddSingleton<IEmailSender, SmtpEmailSender.SmtpEmailSender>();
        return services;
    }

}
