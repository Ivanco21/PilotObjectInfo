using System.Collections.ObjectModel;
using Ascon.Pilot.SDK;

namespace PilotObjectInfo.ViewModels
{
    class AccessViewModel: Base.ViewModel
	{
		private ReadOnlyCollection<IAccessRecord> _access2;

		public AccessViewModel(ReadOnlyCollection<IAccessRecord> access2)
		{
			_access2 = access2;
		}

		public ReadOnlyCollection<IAccessRecord> Access => _access2;
	}
}
