using System;
using System.Collections.Generic;
using Ascon.Pilot.SDK;
using PilotObjectInfo.ViewModels.Commands;

namespace PilotObjectInfo.ViewModels
{
    class ChildrenViewModel :  Base.ViewModel
    {
        private IEnumerable<Guid> _children;
        private IObjectsRepository _objectsRepository;
        private FileModifier _fileModifier;
        private IFileProvider _fileProvider;
        private ITabServiceProvider _tabServiceProvider;
        private RelayCommand _showInfoCmd;

        public ChildrenViewModel(IEnumerable<Guid> children, IObjectsRepository objectsRepository, IFileProvider fileProvider, ITabServiceProvider tabServiceProvider, FileModifier fileModifier)
        {
            _children = children;
            _objectsRepository = objectsRepository;
            _fileModifier = fileModifier;
            _fileProvider = fileProvider;
            _tabServiceProvider = tabServiceProvider;
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
            DialogService.ShowInfo(id, _objectsRepository, _fileProvider, _tabServiceProvider, _fileModifier);
        }
    }
}
