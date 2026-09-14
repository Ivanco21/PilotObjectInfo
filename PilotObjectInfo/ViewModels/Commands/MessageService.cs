using System;

namespace PilotObjectInfo.ViewModels.Commands
{
    public class MessageService
    {
        public Action<string> ShowMessageAction { get; set; }

        public void SendMessage(string message)
        {
            ShowMessageAction?.Invoke(message);
        }
    }
}
