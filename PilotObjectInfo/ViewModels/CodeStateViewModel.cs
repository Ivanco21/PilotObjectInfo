using PilotObjectInfo.Core.DeepAnalytics;
using PilotObjectInfo.Models;
using PilotObjectInfo.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PilotObjectInfo.ViewModels
{
    internal class CodeStateViewModel : Base.ViewModel
    {
        private MessageService _messageService;
        public CodeStateViewModel(MessageService messageService)
        {
            CodeStates = new ObservableCollection<CodeStateModel>();
            IsInProgress = false;
            this._messageService = messageService;

            #region Commands
            this.SearchCommand = new AsyncRelayCommand(OnSearchExecutedAsync, CanSearchExecute);
            this.CodeCopyCommand = new RelayCommand(OnCodeCopyExecuted, CanCodeCopyExecute);
            this.OpenExtFolderCommand = new RelayCommand(OnOpenExtFolderExecuted, CanOpenExtFolderExecute);
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
        public ICommand SearchCommand { get; set; }
        private async Task OnSearchExecutedAsync(object obj)
        {
            IsInProgress = true;

            AssemblyParser parser = new();
            List<string> paths = parser.GetLoadedDllPaths();
            var loadTasks = new List<Task<CodeStateModel>>(paths.Count);
            var throttler = new SemaphoreSlim(initialCount: Environment.ProcessorCount * 2);

            foreach (string pth in paths)
            {
                await throttler.WaitAsync();
                loadTasks.Add(Task.Run(() =>
                {
                    try
                    {
                        AssemblyParser pr = new();
                        return pr.GetCodeState(pth, SearchTerm);
                    }
                    finally
                    {
                        throttler.Release();
                    }
                }));
            }

            CodeStateModel[] codeStates = await Task.WhenAll(loadTasks);

            List<CodeStateModel> relevantCodeStates = codeStates.Where(st => st.CodeParts.Count > 0).ToList();

            CodeStates = new ObservableCollection<CodeStateModel>(relevantCodeStates);

            IsInProgress = false;
        }

        private bool CanSearchExecute(object obj)
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
            _messageService.SendMessage("Copied to clipboard!");
        }

        private bool CanCodeCopyExecute(object obj)
        {
            return true;
        }
        #endregion

        #region Команда открытия папки с Ext
        public ICommand OpenExtFolderCommand { get; set; }
        private void OnOpenExtFolderExecuted(object obj)
        {
            string pathToExt = (string)obj;
            string dirPath = Path.GetDirectoryName(pathToExt);
            Process.Start("explorer.exe", dirPath);
        }

        private bool CanOpenExtFolderExecute(object obj)
        {
            return true;
        }
        #endregion

    }
}
