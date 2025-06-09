using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using PilotObjectInfo.Core.DeepAnalytics;
using PilotObjectInfo.Models;
using PilotObjectInfo.ViewModels.Commands;

namespace PilotObjectInfo.ViewModels
{
    internal class CodeSearchViewModel : Base.ViewModel
    {
        public CodeSearchViewModel()
        {
            CodeStates = new ObservableCollection<CodeStateModel>();

            #region Commands
            this.CodeSearchCommand = new RelayCommand(OnCodeSearchExecuted, CanCodeSearchExecute);
            #endregion
        }

        #region Текст поиска
        private string _searchTerm;
        public string SearchTerm
        {
            get => _searchTerm;
            set => Set(ref _searchTerm, value);
        }
        #endregion

        #region Результаты поиска по коду
        private ObservableCollection<CodeStateModel> _сodeStates;
        public ObservableCollection<CodeStateModel> CodeStates
        {
            get => _сodeStates;
            set => Set(ref _сodeStates, value);
        }
        #endregion

        #region Команда для поиска
        public ICommand CodeSearchCommand { get; set; }
        private void OnCodeSearchExecuted(object obj)
        {
            AssemblyParser parser = new();
            List<CodeStateModel> codeStates = new();

            List<string> paths = parser.GetLoadedDllPaths();
            foreach (var pth in paths)
            {
                CodeStateModel codeState = parser.GetCodeState(pth, SearchTerm);
                if (codeState.CodeParts.Count != 0)
                {
                    codeStates.Add(codeState);
                }
            }

            CodeStates = new ObservableCollection<CodeStateModel>(codeStates);
        }

        private bool CanCodeSearchExecute(object obj)
        {
            return !string.IsNullOrEmpty(SearchTerm);
        }
        #endregion

    }
}
