using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace MealPlanner.Api.Services;

public sealed class EmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public Task SendAccountVerificationAsync(string email, string verificationLink)
    {
        const string subject = "Activa tu cuenta de Meal Planner";
        var body = $"""
            Hola,

            Para activar tu cuenta de Meal Planner, abre este enlace:
            {verificationLink}

            Si no has creado esta cuenta, puedes ignorar este correo.
            """;

        return SendAsync(email, subject, body, "Enlace de activacion para {Email}: {Link}", verificationLink);
    }

    public Task SendPasswordResetAsync(string email, string resetLink)
    {
        const string subject = "Recuperacion de password de Meal Planner";
        var body = $"""
            Hola,

            Para cambiar tu password de Meal Planner, abre este enlace:
            {resetLink}

            Si no has solicitado este cambio, puedes ignorar este correo.
            """;

        return SendAsync(email, subject, body, "Enlace de recuperacion para {Email}: {Link}", resetLink);
    }

    private async Task SendAsync(string email, string subject, string body, string devLogTemplate, string link)
    {
        var host = _configuration["SMTP_HOST"];
        if (string.IsNullOrWhiteSpace(host))
        {
            _logger.LogInformation(devLogTemplate, email, link);
            return;
        }

        var port = int.TryParse(_configuration["SMTP_PORT"], out var configuredPort) ? configuredPort : 587;
        var username = _configuration["SMTP_USERNAME"];
        var password = _configuration["SMTP_PASSWORD"];
        var from = _configuration["SMTP_FROM"];
        var allowInvalidCertificates = bool.TryParse(_configuration["SMTP_ALLOW_INVALID_CERTIFICATES"], out var allowInvalid)
            && allowInvalid;

        if (string.IsNullOrWhiteSpace(from))
        {
            from = string.IsNullOrWhiteSpace(username) ? "no-reply@meal-planner.local" : username;
        }

        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(from));
        message.To.Add(MailboxAddress.Parse(email));
        message.Subject = subject;
        message.Body = new TextPart("plain") { Text = body };

        using var client = new SmtpClient();
        if (allowInvalidCertificates)
        {
            _logger.LogWarning("SMTP_ALLOW_INVALID_CERTIFICATES esta activo. Usar solo en desarrollo local.");
            client.ServerCertificateValidationCallback = AcceptSmtpCertificateForDevelopment;
        }

        await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);

        if (!string.IsNullOrWhiteSpace(username))
        {
            await client.AuthenticateAsync(username, password ?? string.Empty);
        }

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    private static bool AcceptSmtpCertificateForDevelopment(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors) => true;
}
