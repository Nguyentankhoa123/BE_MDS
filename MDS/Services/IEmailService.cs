using MDS.Shared.Core.Helper;

namespace MDS.Services
{
    public interface IEmailService
    {
        void SendEmail(Message message);
    }
}
