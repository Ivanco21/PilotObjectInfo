using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PilotObjectInfo.Models
{
    internal sealed class StateMachineVertex : INotifyPropertyChanged
    {
        private bool _isSelected;

        public StateMachineVertex(Guid id, string name, string title)
        {
            Id = id;
            Name = name ?? string.Empty;
            Title = title ?? string.Empty;
        }

        public Guid Id { get; }

        public string Name { get; }

        public string Title { get; }

        public string DisplayText => string.IsNullOrWhiteSpace(Title) ? Name : Title;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value)
                    return;
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString() => DisplayText;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
