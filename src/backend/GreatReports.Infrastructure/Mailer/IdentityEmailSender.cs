using Microsoft.AspNetCore.Identity;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.ApplicationJobs;
using GreatReports.Infrastructure.Identity;

namespace GreatReports.Infrastructure.Mailer;

public class IdentityEmailSender : IEmailSender<Account>
{
    private readonly IBackgroundJobService _backgroundJobService;

    public IdentityEmailSender(IBackgroundJobService backgroundJobService)
    {
        _backgroundJobService = backgroundJobService;
    }

    public Task SendConfirmationLinkAsync(Account user, string email, string confirmationLink)
    {
        var subject = "Confirme seu endereço de e-mail";
        var body = $"""
            <div style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 8px;">
                <h2 style="color: #333333; text-align: center;">Bem-vindo ao Great Reports!</h2>
                <p style="font-size: 16px; color: #555555; line-height: 1.5;">
                    Olá, obrigado por se cadastrar. Para concluir a configuração da sua conta, por favor confirme seu endereço de e-mail clicando no botão abaixo:
                </p>
                <div style="text-align: center; margin: 30px 0;">
                    <a href="{confirmationLink}" style="background-color: #007bff; color: #ffffff; padding: 12px 24px; text-decoration: none; border-radius: 4px; font-weight: bold; display: inline-block;">Confirmar E-mail</a>
                </div>
                <p style="font-size: 14px; color: #777777; line-height: 1.5;">
                    Se o botão acima não funcionar, copie e cole o seguinte link no seu navegador: <br/>
                    <a href="{confirmationLink}" style="color: #007bff; word-break: break-all;">{confirmationLink}</a>
                </p>
                <hr style="border: 0; border-top: 1px solid #e0e0e0; margin: 20px 0;" />
                <p style="font-size: 12px; color: #999999; text-align: center;">
                    Este é um e-mail automático, por favor não responda.
                </p>
            </div>
            """;

        _backgroundJobService.Enqueue<SendEmailJob>(job => job.ExecuteAsync(email, subject, body, CancellationToken.None));
        return Task.CompletedTask;
    }

    public Task SendPasswordResetLinkAsync(Account user, string email, string resetLink)
    {
        var subject = "Recuperação de senha";
        var body = $"""
            <div style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 8px;">
                <h2 style="color: #333333; text-align: center;">Recuperação de Senha</h2>
                <p style="font-size: 16px; color: #555555; line-height: 1.5;">
                    Você solicitou a redefinição de senha para a sua conta do Great Reports. Clique no botão abaixo para escolher uma nova senha:
                </p>
                <div style="text-align: center; margin: 30px 0;">
                    <a href="{resetLink}" style="background-color: #28a745; color: #ffffff; padding: 12px 24px; text-decoration: none; border-radius: 4px; font-weight: bold; display: inline-block;">Redefinir Senha</a>
                </div>
                <p style="font-size: 14px; color: #777777; line-height: 1.5;">
                    Se você não fez essa solicitação, pode ignorar este e-mail com segurança.
                </p>
                <p style="font-size: 14px; color: #777777; line-height: 1.5;">
                    Se o botão acima não funcionar, copie e cole o seguinte link no seu navegador: <br/>
                    <a href="{resetLink}" style="color: #28a745; word-break: break-all;">{resetLink}</a>
                </p>
                <hr style="border: 0; border-top: 1px solid #e0e0e0; margin: 20px 0;" />
                <p style="font-size: 12px; color: #999999; text-align: center;">
                    Este é um e-mail automático, por favor não responda.
                </p>
            </div>
            """;

        _backgroundJobService.Enqueue<SendEmailJob>(job => job.ExecuteAsync(email, subject, body, CancellationToken.None));
        return Task.CompletedTask;
    }

    public Task SendPasswordResetCodeAsync(Account user, string email, string resetCode)
    {
        var subject = "Código para recuperação de senha";
        var body = $"""
            <div style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 8px;">
                <h2 style="color: #333333; text-align: center;">Código de Recuperação de Senha</h2>
                <p style="font-size: 16px; color: #555555; line-height: 1.5;">
                    Para prosseguir com a redefinição de sua senha, utilize o código de segurança abaixo no aplicativo:
                </p>
                <div style="text-align: center; margin: 30px 0; background-color: #f8f9fa; padding: 15px; border-radius: 4px; font-size: 24px; font-weight: bold; letter-spacing: 2px; color: #333333; border: 1px dashed #cccccc;">
                    {resetCode}
                </div>
                <p style="font-size: 14px; color: #777777; line-height: 1.5;">
                    Se você não solicitou este código, ignore este e-mail.
                </p>
                <hr style="border: 0; border-top: 1px solid #e0e0e0; margin: 20px 0;" />
                <p style="font-size: 12px; color: #999999; text-align: center;">
                    Este é um e-mail automático, por favor não responda.
                </p>
            </div>
            """;

        _backgroundJobService.Enqueue<SendEmailJob>(job => job.ExecuteAsync(email, subject, body, CancellationToken.None));
        return Task.CompletedTask;
    }
}
