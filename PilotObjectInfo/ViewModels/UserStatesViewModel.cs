using Ascon.Pilot.SDK;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PilotObjectInfo.ViewModels
{
    class UserStatesViewModel : Base.ViewModel
    {
        public UserStatesViewModel(IEnumerable<IUserState> states)
        {
            States = new ObservableCollection<IUserState>(states);
        }

        public ObservableCollection<IUserState> States { get; }
    }
}
