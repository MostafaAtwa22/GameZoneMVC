using GameZone.Core.Models;

namespace GameZone.Services.Services.EmailServices
{
    public interface IEmailSettings
    {
        void SendEmail(Email email);
    }
}
