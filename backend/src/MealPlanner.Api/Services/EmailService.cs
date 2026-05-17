using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Net.Security;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
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
        var resendApiKey = _configuration["RESEND_API_KEY"];
        if (!string.IsNullOrWhiteSpace(resendApiKey))
        {
            await SendWithResendAsync(resendApiKey, email, subject, body);
            return;
        }

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
        var timeoutSeconds = int.TryParse(_configuration["SMTP_TIMEOUT_SECONDS"], out var configuredTimeout)
            ? Math.Clamp(configuredTimeout, 5, 60)
            : 15;
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
        client.Timeout = timeoutSeconds * 1000;

        if (allowInvalidCertificates)
        {
            _logger.LogWarning("SMTP_ALLOW_INVALID_CERTIFICATES esta activo. Usar solo en desarrollo local.");
            client.ServerCertificateValidationCallback = AcceptSmtpCertificateForDevelopment;
        }

        var socketOptions = port == 465
            ? SecureSocketOptions.SslOnConnect
            : SecureSocketOptions.StartTls;

        await client.ConnectAsync(host, port, socketOptions);

        if (!string.IsNullOrWhiteSpace(username))
        {
            await client.AuthenticateAsync(username, password ?? string.Empty);
        }

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    private static bool AcceptSmtpCertificateForDevelopment(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors) => true;

    private async Task SendWithResendAsync(string apiKey, string email, string subject, string body)
    {
        var from = _configuration["EMAIL_FROM"];
        if (string.IsNullOrWhiteSpace(from))
        {
            from = _configuration["SMTP_FROM"];
        }

        if (string.IsNullOrWhiteSpace(from))
        {
            from = "Meal Planner <onboarding@resend.dev>";
        }

        using var client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(15)
        };

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var payload = new
        {
            from,
            to = new[] { email },
            subject,
            text = body
        };

        using var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json");

        using var response = await client.PostAsync("https://api.resend.com/emails", content);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("Resend devolvio {StatusCode}: {Body}", response.StatusCode, error);
            throw new InvalidOperationException("No se pudo enviar el correo con Resend.");
        }
    }
}
