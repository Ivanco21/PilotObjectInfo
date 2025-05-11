using Ascon.Pilot.SDK;
using PilotObjectInfo.Core;
using PilotObjectInfo.Core.Services;
using PilotObjectInfo.ViewModels.Commands;
using System;
using System.Windows.Input;

namespace PilotObjectInfo.ViewModels
{
    class MainViewModel : Base.ViewModel
    {
        private IDataObject _obj;

        private RelayCommand _goToCommand;

        public MainViewModel(IDataObject obj, FileModifier fileModifier)
        {
            _obj = obj;

            AttributesVm = new AttributesViewModel(_obj);
            TypeVm = new TypeViewModel(_obj.Type);
            CreatorVm = new CreatorViewModel(_obj.Creator);
            FilesVm = new FilesViewModel(obj.Id, _obj.Files, fileModifier);
            SnapshotsVm = new SnapshotsViewModel(_obj.Id, _obj.PreviousFileSnapshots);

            AccessVm = new AccessViewModel(_obj.Access2);
            RelationsVm = new RelationsViewModel(obj.Relations, fileModifier);
            StateInfoVm = new StateInfoViewModel(obj.ObjectStateInfo);
            ChildrenVm = new ChildrenViewModel(obj.Children, fileModifier);
            PeopleVm = new PeopleViewModel(GI.Repository.GetPeople());
            OrgUnitsVm = new OrgUnitsViewModel(GI.Repository.GetOrganisationUnits());
            TypesVm = new TypesViewModel(GI.Repository.GetTypes());
            UserStatesVm = new UserStatesViewModel(GI.Repository.GetUserStates());

            GI.Repository.GetOrganisationUnits();

            #region Commands
            this.GoToDataObjectCommand = new RelayCommand(OnGoToDataObjectExecuted, CanGoToDataObjectExecute);
            #endregion
        }

        public Guid Id => _obj.Id;

        public string DisplayName => _obj.DisplayName;

        public DateTime Created => _obj.Created;

        public bool IsSecret => _obj.IsSecret;

        public Guid ParentId => _obj.ParentId;

        public int CurrentUserId => GI.Repository.GetCurrentPerson().Id;

        public AttributesViewModel AttributesVm { get; }
        public TypeViewModel TypeVm { get; }
        public CreatorViewModel CreatorVm { get; }
        public FilesViewModel FilesVm { get; }
        public SnapshotsViewModel SnapshotsVm { get; }
        public AccessViewModel AccessVm { get; }
        public RelationsViewModel RelationsVm { get; }
        public StateInfoViewModel StateInfoVm { get; }
        public ChildrenViewModel ChildrenVm { get; }
        public PeopleViewModel PeopleVm { get; }
        public OrgUnitsViewModel OrgUnitsVm { get; }
        public UserStatesViewModel UserStatesVm { get; }

        public TypesViewModel TypesVm { get; }


        #region Команда Переход к объекту
        public ICommand GoToDataObjectCommand { get; set; }
        private void OnGoToDataObjectExecuted(object obj)
        {
            Guid id = (Guid)obj;
            GI.TabServiceProvider.ShowElement(id);
        }
        private bool CanGoToDataObjectExecute(object obj)
        {
            return obj != null && obj is Guid;
        }
        #endregion
    }
}
