using eventify_backend.Models;

namespace eventify_backend.UtilityService
{
    public interface IEmailService
    {
        void SendEmail(Email email);

    }
}
