using System.Collections.Generic;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Ascon.Pilot.SDK;
using PilotObjectInfo.ViewModels.Commands;

namespace PilotObjectInfo.ViewModels
{
    class TypesViewModel: Base.ViewModel
    {

        public TypesViewModel(IEnumerable<IType> types)
        {
            Types = new ObservableCollection<IType>(types);
            TypesView = CollectionViewSource.GetDefaultView(Types);
            TypesView.Filter = FilterTypes;
            this.SearchCommand = new RelayCommand(OnSearchExecuted, CanSearchExecute);
        }
        public ObservableCollection<IType> Types { get; }

        #region Поиск - View
        private ICollectionView _typesView;
        public ICollectionView TypesView
        {
            get => _typesView;
            set => Set(ref _typesView, value);
        }
        #endregion

        #region Поиск - фильтр
        private bool FilterTypes(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
                return true;

            if (obj is IType type)
            {
                return type.Name.ToLower().Contains(SearchText.Trim().ToLower());
            }

            return false;
        }
        #endregion

        #region Поиск - значение
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                Set(ref _searchText, value);
                TypesView.Refresh();
            }
        }
        #endregion

        #region Команда для поиска
        public ICommand SearchCommand { get; set; }
        private void OnSearchExecuted(object obj)
        {
            TypesView.Refresh();
        }
        private bool CanSearchExecute(object obj)
        {
            return true;
        }
        #endregion
    }
}
