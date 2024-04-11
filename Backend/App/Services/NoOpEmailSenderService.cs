using Gemeinschaftsgipfel.Models;
using Microsoft.AspNetCore.Identity;

namespace Gemeinschaftsgipfel.Services;

public class NoOpEmailSenderService : IEmailSender<User>
{
    public async Task SendConfirmationLinkAsync(User user, string email, string confirmationLink)
    {
        await Task.Run(() => { });
    }

    public async Task SendPasswordResetLinkAsync(User user, string email, string resetLink)
    {
        await Task.Run(() => { });
    }

    public async Task SendPasswordResetCodeAsync(User user, string email, string resetCode)
    {
        await Task.Run(() => { });
    }
}