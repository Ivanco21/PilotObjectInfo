using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using PilotObjectInfo.Core.DeepAnalytics;
using PilotObjectInfo.Core.Services;
using PilotObjectInfo.Models;
using PilotObjectInfo.ViewModels.Commands;

namespace PilotObjectInfo.ViewModels
{
    class AnalyticsViewModel : Base.ViewModel
    {
        PilotDataMapper pMapper = new();
        public AnalyticsViewModel()
        {
            _assemblyDescriptions = new ObservableCollection<AssemblyInfoModel>();
            QPilot qPilot = new();
            pMapper.TypesPilot = qPilot.GetTypeAllModels();
            pMapper.AttributesPilot = qPilot.GetAttributeAllModels();
            pMapper.UserStatesPilot = qPilot.GetUserStateAllModels();


            #region Commands
            this.SearchAssemblyCommand = new RelayCommand(OnSearchAssemblyExecuted, CanSearchAssemblyExecute);
            #endregion
        }

        #region Команда для поиска
        public ICommand SearchAssemblyCommand{ get; set; }
        private void OnSearchAssemblyExecuted(object obj)
        {
            List<AssemblyInfoModel> infos = new();
            AssemblyParser parser = new();
            List<string> paths = parser.GetLoadedDllPaths();
            foreach (var pth in paths)
            {
                var raw = parser.DoParse(pth);
                if (raw.Strings.Count == 0)
                {
                    continue;
                }

                var assemblyInfo = pMapper.GetAssemblyInfoModel(raw);
                infos.Add(assemblyInfo);
            }

            AssemblyDescriptions = new ObservableCollection<AssemblyInfoModel>(infos);
        }
        private bool CanSearchAssemblyExecute(object obj)
        {
            return true;
        }
        #endregion

        #region Assembly description
        private ObservableCollection<AssemblyInfoModel> _assemblyDescriptions;
        public ObservableCollection<AssemblyInfoModel> AssemblyDescriptions
        {
            get => _assemblyDescriptions;
            set => Set(ref _assemblyDescriptions, value);
        }
        #endregion

        #region Selected assembly
        private AssemblyInfoModel _selectedAssembly;
        public AssemblyInfoModel SelectedAssembly
        {
            get => _selectedAssembly;
            set
            {
                Set(ref _selectedAssembly, value);
                if (_selectedAssembly != null)
                {
                    AttributeDescriptions = new ObservableCollection<AttributeModel>(_selectedAssembly.Attributes.OrderBy(a => a.Name));
                    TypeDescriptions = new ObservableCollection<TypeModel>(_selectedAssembly.Types.OrderBy(t => t.Name));
                    UserStateDescriptions = new ObservableCollection<UserStateModel>(_selectedAssembly.UserStates.OrderBy(t => t.Name));
                }
            }
        }
        #endregion

        #region Описание типов assembly
        private ObservableCollection<TypeModel> _typeDescriptions;
        public ObservableCollection<TypeModel> TypeDescriptions
        {
            get => _typeDescriptions;
            set => Set(ref _typeDescriptions, value);
        }
        #endregion

        #region Описание атрибутов assembly
        private ObservableCollection<AttributeModel> _attributeDescriptions;
        public ObservableCollection<AttributeModel> AttributeDescriptions
        {
            get => _attributeDescriptions;
            set => Set(ref _attributeDescriptions, value);
        }
        #endregion

        #region Описание состояний
        private ObservableCollection<UserStateModel> _userStateDescriptions;
        public ObservableCollection<UserStateModel> UserStateDescriptions
        {
            get => _userStateDescriptions;
            set => Set(ref _userStateDescriptions, value);
        }
        #endregion
    }
}
