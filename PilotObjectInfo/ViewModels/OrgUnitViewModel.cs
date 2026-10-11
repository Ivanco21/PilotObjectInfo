using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Ascon.Pilot.SDK;
using PilotObjectInfo.Core;

namespace PilotObjectInfo.ViewModels
{
    class OrgUnitViewModel
    {
        private IOrganisationUnit _organisationUnit;

        public OrgUnitViewModel(IEnumerable<IOrganisationUnit> organisationUnits, IOrganisationUnit organisationUnit)
        {
            _organisationUnit = organisationUnit;
            var children = organisationUnit.Children.Select(x => organisationUnits.FirstOrDefault(y => y.Id == x)).Where(x => x != null);
            Children = new ObservableCollection<OrgUnitViewModel>(children.Select(x=> new OrgUnitViewModel(organisationUnits, x)));

        }

        public OrgUnitViewModel(IOrganisationUnit organisationUnit)
        {
            _organisationUnit = organisationUnit;
            Children = new ObservableCollection<OrgUnitViewModel>();
        }

        public int Id => _organisationUnit.Id;

        public string Title => _organisationUnit.Title;

        public string PersonDisplayName => _organisationUnit.IsPosition ? GI.Repository.GetPerson(_organisationUnit.Person()).DisplayName : string.Empty;

        public bool IsDeleted => _organisationUnit.IsDeleted;

        public bool IsPosition => _organisationUnit.IsPosition;

        public bool IsChief => _organisationUnit.IsChief;


        public ObservableCollection<OrgUnitViewModel> Children { get; }
    }
}
