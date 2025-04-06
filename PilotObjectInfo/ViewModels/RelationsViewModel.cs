using System;
using System.Collections.ObjectModel;
using Ascon.Pilot.SDK;
using PilotObjectInfo.ViewModels.Commands;

namespace PilotObjectInfo.ViewModels
{
    class RelationsViewModel : Base.ViewModel
    {
		private ReadOnlyCollection<IRelation> _relations;
        private IObjectsRepository _objectsRepository;
        private IFileProvider _fileProvider;
        private ITabServiceProvider _tabServiceProvider;
        private FileModifier _fileModifier;
        private RelayCommand _showInfoCmd;

        public RelationsViewModel(ReadOnlyCollection<IRelation> relations,
            IObjectsRepository objectsRepository, 
            IFileProvider fileProvider,
            ITabServiceProvider tabServiceProvider,
            FileModifier fileModifier)
		{
			_relations = relations;
            _objectsRepository = objectsRepository;
            _fileProvider = fileProvider;
            _tabServiceProvider = tabServiceProvider;
            _fileModifier = fileModifier;


        }

		public ReadOnlyCollection<IRelation> Relations => _relations;

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
