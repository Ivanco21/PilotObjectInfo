using System.ComponentModel;
using System.Runtime.CompilerServices;
using QuikGraph;

namespace PilotObjectInfo.Models
{
    internal sealed class StateMachineEdge : Edge<StateMachineVertex>, INotifyPropertyChanged
    {
        private bool _isSelected;

        public StateMachineEdge(
            StateMachineVertex source,
            StateMachineVertex target,
            string displayName,
            bool isBidirectional = false)
            : base(source, target)
        {
            DisplayName = displayName ?? string.Empty;
            IsBidirectional = isBidirectional;
        }

        public string DisplayName { get; }

        /// <summary>
        /// True when transitions exist in both directions; drawn with arrows on both ends.
        /// </summary>
        public bool IsBidirectional { get; }

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

        public override string ToString() => DisplayName;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
