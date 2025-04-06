using Ascon.Pilot.SDK;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PilotObjectInfo.ViewModels
{
    class TypesViewModel: Base.ViewModel
    {
        public TypesViewModel(IEnumerable<IType> types)
        {
            Types = new ObservableCollection<IType>(types);
        }

        public ObservableCollection<IType> Types { get; }
    }
}
