using Ascon.Pilot.SDK;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using PilotObjectInfo.ViewModels.Commands;

namespace PilotObjectInfo.ViewModels
{
    class PeopleViewModel : Base.ViewModel
    {
        public PeopleViewModel(IEnumerable<IPerson> people)
        {
            People = new ObservableCollection<IPerson>(people);
            PeopleView = CollectionViewSource.GetDefaultView(People);
            PeopleView.Filter = FilterPeople;

            this.SearchCommand = new RelayCommand(OnSearchExecuted, CanSearchExecute);
        }

       public ObservableCollection<IPerson> People { get; }

        #region Поиск - View
        private ICollectionView _peopleView;
        public ICollectionView PeopleView
        {
            get => _peopleView;
            set => Set(ref _peopleView, value);
        }
        #endregion


        #region Поиск - значение
        private string _searchTerm;
        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                Set(ref _searchTerm, value);
                PeopleView.Refresh();
            }
        }
        #endregion

        #region Команда для поиска
        public ICommand SearchCommand { get; set; }
        private void OnSearchExecuted(object obj)
        {
            PeopleView.Refresh();
        }
        private bool CanSearchExecute(object obj)
        {
            return true;
        }
        #endregion

        #region Поиск - фильтр
        private bool FilterPeople(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
                return true;

            string search = SearchTerm.Trim().ToLower();
            if (obj is IPerson type)
            {
                return type.Id.ToString() == search ||
                       type.Login.ToLower().Contains(search) ||
                       type.DisplayName.ToLower().Contains(search) ||
                       CheckMainPosition(type).Contains(search) ||
                       type.ActualName.ToLower().Contains(search) ||
                       type.Sid.ToLower().Contains(search);
            }
            return false;
        }

        private string CheckMainPosition(IPerson type)
        {
            if (type.MainPosition == null)
                return string.Empty;
            return type.MainPosition.Position.ToString().ToLower();
        }
        #endregion
    }
}
