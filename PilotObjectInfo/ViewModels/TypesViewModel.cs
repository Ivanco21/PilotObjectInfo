using System.Collections.Generic;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System.Reactive.Linq;
using System.Collections.ObjectModel;
using Ascon.Pilot.SDK;
using PilotObjectInfo.ViewModels.Commands;
using PilotObjectInfo.Models;
using System.Windows.Navigation;

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

            string search = SearchText.Trim().ToLower();
            if (obj is IType type)
            {
                return type.Name.ToLower().Contains(search) || 
                       type.Attributes.Any(a => a.Name.ToLower().Contains(search));
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

        #region Selected type
        private IType _selectedType;
        public IType SelectedType
        {
            get => _selectedType;
            set
            {
                Set(ref _selectedType, value);
                if (_selectedType != null)
                {
                    var tmp = _selectedType.Attributes.Select(a => new AttributeDescriptionModel(a));
                    AttributeDescriptions = new ObservableCollection<AttributeDescriptionModel>(tmp);
                }
            }
        }
        #endregion
        #region Описание атрибутов типа
        private ObservableCollection<AttributeDescriptionModel> _attributeDescriptions;
        public ObservableCollection<AttributeDescriptionModel> AttributeDescriptions
        {
            get => _attributeDescriptions;
            set => Set(ref _attributeDescriptions, value);
        }
        #endregion
    }
}
