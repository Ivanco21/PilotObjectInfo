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
using PilotObjectInfo.Core.Services;
using System.Threading.Tasks;
using System;
using PilotObjectInfo.Core;
using System.Windows;
using PilotObjectInfo.Settings;

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
            this.GoToRandomTypeElementCommand = new AsyncRelayCommand(OnGoToRandomTypeElementExecutedAsync, CanGoToRandomTypeElementExecute);
            this.GenerateFullDtoCommand = new RelayCommand(OnGenerateFullDtoExecuted, CanGenerateFullDtoExecute);
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
            if (string.IsNullOrWhiteSpace(SearchTerm))
                return true;

            string search = SearchTerm.Trim().ToLower();
            if (obj is IType type)
            {
                return type.Name.ToLower().Contains(search) || 
                       type.Title.ToLower().Contains(search) ||
                       type.Attributes.Any(a => a.Name.ToLower().Contains(search)) ||
                       type.Attributes.Any(a => a.Title.ToLower().Contains(search));

            }

            return false;
        }
        #endregion


        #region Текст поиска
        private string _searchTerm;
        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                Set(ref _searchTerm, value);
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

        #region Команда "показать случайный элемент типа"
        public ICommand GoToRandomTypeElementCommand { get; set; }

        private async Task OnGoToRandomTypeElementExecutedAsync(object obj)
        {
            QPilot searchService = new();

            if (obj is not IType type)
                return;
            
            IEnumerable<Guid> guids = await searchService.SearchObjectsByTypesAsync(QSettings.MAX_Q_ELEMENTS, type.Id);
            IEnumerable<Guid> validGuids = guids.Where(g => g != Guid.Empty);
            if (!guids.Any())
            {
                return;
            }

            bool isElementFound = false;
            int maxSearchCount = 100;//попыток загрузки
            int iSearch = 0;

            while(!isElementFound && iSearch < maxSearchCount)
            {
                Random seededRandom = new();
                int rnd = seededRandom.Next(validGuids.Count() - 1);
                Guid rndGuid = guids.ElementAt(rnd);
                try
                {
                    GI.TabServiceProvider.ShowElement(rndGuid, true);
                    isElementFound = true;
                }
                catch (Exception)
                {

                }
                iSearch++;
            }

            if(!isElementFound)
            {
                throw new Exception($"Не удалось найти элемент типа - {type.Name} за {maxSearchCount} итераций!");
            }

        }

        private bool CanGoToRandomTypeElementExecute(object obj)
        {
            return SelectedType != null;
        }
        #endregion

        #region Команда "генерация DTO"
        public ICommand GenerateFullDtoCommand { get; set; }
        private void OnGenerateFullDtoExecuted(object obj)
        {
            if (obj is not IType type)
                return;

            DtoGeneratorService dtoGeneratorService = new();
            var dto = dtoGeneratorService.GenerateFullDTO(type);
            Clipboard.SetText(dto.ToString());
        }
        private bool CanGenerateFullDtoExecute(object obj)
        {
            return SelectedType != null;
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
                    var tmp = _selectedType.Attributes.Select(a => new AttributeModel(a));
                    AttributeDescriptions = new ObservableCollection<AttributeModel>(tmp);
                }
            }
        }
        #endregion

        #region Описание атрибутов типа
        private ObservableCollection<AttributeModel> _attributeDescriptions;
        public ObservableCollection<AttributeModel> AttributeDescriptions
        {
            get => _attributeDescriptions;
            set => Set(ref _attributeDescriptions, value);
        }
        #endregion
    }
}
