using System;
using System.Windows.Threading;
using PilotObjectInfo.ViewModels.Commands;

namespace PilotObjectInfo.ViewModels
{
    class FooterViewModel : Base.ViewModel
    {
        #region Статус 
        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set => Set(ref _statusMessage, value);
        }
        private DispatcherTimer _messageTimer;

        public FooterViewModel(MessageService messageService)
        {
            messageService.ShowMessageAction = ShowTemporaryMessage;
        }

        public void ShowTemporaryMessage(string message)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(() =>
            {
                StatusMessage = message;

                _messageTimer?.Stop();
                _messageTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(5000)
                };

                _messageTimer.Tick += (s, e) =>
                {
                    StatusMessage = string.Empty;
                    _messageTimer.Stop();
                };

                _messageTimer.Start();
            });
        }
        #endregion
    }
}
