using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Ascon.Pilot.SDK;
using PilotObjectInfo.Core;
using PilotObjectInfo.ViewModels.Commands;

namespace PilotObjectInfo.ViewModels
{
    class OrgUnitsViewModel : Base.ViewModel
    {
        private readonly List<OrgUnitViewModel> _roots;
        private readonly List<OrgUnitSearchEntry> _searchEntries;

        public OrgUnitsViewModel(IEnumerable<IOrganisationUnit> organisationUnits)
        {
            var units = organisationUnits.ToList();
            var peopleNames = BuildPeopleNames();

            _roots = units.Where(x => x.Id == 0).Select(x => new OrgUnitViewModel(units, x)).ToList();
            _searchEntries = units
                .Select(unit => new OrgUnitSearchEntry(new OrgUnitViewModel(unit), GetPersonNames(unit, peopleNames)))
                .ToList();

            OrganisationUnits = new ObservableCollection<OrgUnitViewModel>(_roots);
            SearchCommand = new RelayCommand(OnSearchExecuted, CanSearchExecute);
        }

        public ObservableCollection<OrgUnitViewModel> OrganisationUnits { get; }

        #region Поиск - значение
        private string _searchTerm;
        public string SearchTerm
        {
            get => _searchTerm;
            set => Set(ref _searchTerm, value);
        }
        #endregion

        #region Команда для поиска
        public ICommand SearchCommand { get; set; }
        private void OnSearchExecuted(object obj)
        {
            ApplySearch();
        }
        private bool CanSearchExecute(object obj)
        {
            return true;
        }
        #endregion

        #region Поиск - фильтр
        private void ApplySearch()
        {
            IEnumerable<OrgUnitViewModel> result = _roots;
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                string search = SearchTerm.Trim().ToLower();
                result = _searchEntries
                    .Where(entry => entry.Matches(search))
                    .Select(entry => entry.ViewModel);
            }

            OrganisationUnits.Clear();
            foreach (var unit in result)
                OrganisationUnits.Add(unit);
        }

        private static Dictionary<int, string[]> BuildPeopleNames()
        {
            Dictionary<int, string[]> result = new();
            var people = GI.Repository.GetPeople();
            if (people == null)
                return result;

            foreach (var person in people)
            {
                if (person == null || 
                    result.ContainsKey(person.Id))
                    continue;

                result[person.Id] = new[] { person.DisplayName, person.ActualName }
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .Select(name => name.Trim().ToLower())
                    .Distinct()
                    .ToArray();
            }

            return result;
        }

        private static string[] GetPersonNames(IOrganisationUnit unit, Dictionary<int, string[]> peopleNames)
        {
            var personIds = new List<int>();
            int personId = unit.Person();
            if (personId >= 0)
                personIds.Add(personId);

            AddPersonIds(personIds, unit.VicePersons());
            AddPersonIds(personIds, unit.GroupPersons());
            AddPersonIds(personIds, unit.PermanentVicePersons());

            return personIds
                .Distinct()
                .Where(peopleNames.ContainsKey)
                .SelectMany(id => peopleNames[id])
                .Distinct()
                .ToArray();
        }

        private static void AddPersonIds(List<int> target, IEnumerable<int> personIds)
        {
            if (personIds == null)
                return;

            target.AddRange(personIds.Where(id => id >= 0));
        }
        #endregion

        private sealed class OrgUnitSearchEntry
        {
            private readonly string[] _personNames;

            public OrgUnitSearchEntry(OrgUnitViewModel viewModel, string[] personNames)
            {
                ViewModel = viewModel;
                _personNames = personNames;
            }

            public OrgUnitViewModel ViewModel { get; }

            public bool Matches(string search)
            {
                if (ViewModel.Id.ToString() == search)
                    return true;

                return _personNames.Any(name => name.Contains(search));
            }
        }
    }
}
