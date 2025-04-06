using System;
using Ascon.Pilot.SDK;

namespace PilotObjectInfo.ViewModels
{
    class StateInfoViewModel : Base.ViewModel
    {
        private IStateInfo _stateInfo;

        public StateInfoViewModel(IStateInfo stateInfo)
        {
            _stateInfo = stateInfo;
        }

        public string State => _stateInfo.State.ToString();

        public DateTime? Date => _stateInfo?.Date;

        public int? PersonId => _stateInfo?.PersonId;

        public int? PositionId => _stateInfo?.PositionId;
    }
}
