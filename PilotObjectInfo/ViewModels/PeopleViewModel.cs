using System.Collections.Generic;
using System.Collections.ObjectModel;
using Ascon.Pilot.SDK;

namespace PilotObjectInfo.ViewModels
{
    class PeopleViewModel
    {
        public PeopleViewModel(IEnumerable<IPerson> people)
        {
            People = new ObservableCollection<IPerson>(people);
        }

       public ObservableCollection<IPerson> People { get; }
    }
}
