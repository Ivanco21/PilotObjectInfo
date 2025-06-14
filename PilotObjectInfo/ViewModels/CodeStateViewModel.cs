using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using PilotObjectInfo.Core.DeepAnalytics;
using PilotObjectInfo.Models;
using PilotObjectInfo.ViewModels.Commands;

namespace PilotObjectInfo.ViewModels
{
    internal class CodeStateViewModel : Base.ViewModel
    {
        public CodeStateViewModel()
        {
            CodeStates = new ObservableCollection<CodeStateModel>();
            IsInProgress = false;

            #region Commands
            this.CodeSearchCommand = new AsyncRelayCommand(OnCodeSearchExecutedAsync, CanCodeSearchExecute);
            this.CodeCopyCommand = new RelayCommand(OnCodeCopyExecuted, CanCodeCopyExecute);
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


        #region Задачи в обработке?
        private bool _isInProgress;
        public bool IsInProgress
        {
            get => _isInProgress;
            set => Set(ref _isInProgress, value);
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
        private async Task OnCodeSearchExecutedAsync(object obj)
        {
            IsInProgress = true;

            AssemblyParser parser = new();
            List<string> paths = parser.GetLoadedDllPaths();
            List<Task<CodeStateModel>> loadTasks = new();
            for (var i = 0; i < paths.Count; i++)
            {
                string pth = paths[i];
                loadTasks.Add(Task.Run(() =>
                {
                    AssemblyParser pr = new();
                    CodeStateModel codeState = pr.GetCodeState(pth, SearchTerm);
                    return codeState;
                }));
            }

            CodeStateModel[] codeStates = await Task.WhenAll(loadTasks);
            List<CodeStateModel> relevantCodeStates = codeStates.Where(st => st.CodeParts.Count > 0).ToList();

            CodeStates = new ObservableCollection<CodeStateModel>(relevantCodeStates);

            IsInProgress = false;
        }

        private bool CanCodeSearchExecute(object obj)
        {
            return !string.IsNullOrEmpty(SearchTerm) && SearchTerm.Trim().Length > 0 && !IsInProgress;
        }
        #endregion

        #region Команда копирования кода
        public ICommand CodeCopyCommand { get; set; }
        private void OnCodeCopyExecuted(object obj)
        {
            AssemblyParser parser = new();
            List<string> paths = parser.GetLoadedDllPaths();

            string pathToExt = (string)obj;
            if (! paths.Contains(pathToExt))
            {
                Clipboard.SetText($"Ошибка. Ext - {pathToExt} не найден");
            }

            string rawCSCode = parser.DecompileAssemblyToCSharp(pathToExt);
            Clipboard.SetText(rawCSCode);
        }

        private bool CanCodeCopyExecute(object obj)
        {
            return true;
        }
        #endregion

    }
}
