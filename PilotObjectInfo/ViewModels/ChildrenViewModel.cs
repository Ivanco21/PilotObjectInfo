using System;
using System.Collections.Generic;
using Ascon.Pilot.SDK;
using PilotObjectInfo.ViewModels.Commands;

namespace PilotObjectInfo.ViewModels
{
    class ChildrenViewModel :  Base.ViewModel
    {
        private IEnumerable<Guid> _children;
        private FileModifier _fileModifier;

        private RelayCommand _showInfoCmd;

        public ChildrenViewModel(IEnumerable<Guid> children, FileModifier fileModifier)
        {
            _children = children;
            _fileModifier = fileModifier;
        }

        public IEnumerable<Guid> Children => _children;

        public RelayCommand ShowInfoCmd
        {
            get
            {
                if (_showInfoCmd == null)
                {
                    _showInfoCmd = new RelayCommand(DoShowInfo);
                }
                return _showInfoCmd;
            }
        }

        private void DoShowInfo(object obj)
        {
            Guid id = (Guid)obj;
            DialogService.ShowInfo(id, _fileModifier);
        }
    }
}
