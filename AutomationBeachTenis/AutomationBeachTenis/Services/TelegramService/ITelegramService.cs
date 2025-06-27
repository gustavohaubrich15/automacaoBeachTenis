using System.Text;

namespace AutomationBeachTenis.Services.TelegramService
{
    public interface ITelegramService
    {
        Task SendMessageTelegramToChannel(StringBuilder message);
    }
}
