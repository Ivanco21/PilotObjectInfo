using Ascon.Pilot.SDK;
using PilotObjectInfo.ViewModels.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace PilotObjectInfo.ViewModels
{
    class UserStatesViewModel : Base.ViewModel
    {
        public UserStatesViewModel(IEnumerable<IUserState> states)
        {
            States = new ObservableCollection<IUserState>(states);
            StatesView = CollectionViewSource.GetDefaultView(States);
            StatesView.Filter = FilterStates;

            this.SearchCommand = new RelayCommand(OnSearchExecuted, CanSearchExecute);
        }


        public ObservableCollection<IUserState> States { get; }

        #region Поиск - View
        private ICollectionView _statesView;
        public ICollectionView StatesView
        {
            get => _statesView;
            set => Set(ref _statesView, value);
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
                StatesView.Refresh();
            }
        }
        #endregion

        #region Команда для поиска
        public ICommand SearchCommand { get; set; }
        private void OnSearchExecuted(object obj)
        {
            StatesView.Refresh();
        }
        private bool CanSearchExecute(object obj)
        {
            return true;
        }
        #endregion

        #region Поиск - фильтр
        private bool FilterStates(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
                return true;

            string search = SearchTerm.Trim().ToLower();
            if (obj is IUserState type)
            {
                return type.Id.ToString() == search ||
                       type.Name.ToLower().Contains(search) ||
                       type.Title.ToLower().Contains(search);
            }
            return false;
        }
        #endregion
    }

}
