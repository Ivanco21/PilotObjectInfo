using System.Windows.Input;
using PilotObjectInfo.Core.Services;
using PilotObjectInfo.ViewModels.Commands;

namespace PilotObjectInfo.ViewModels
{
    internal class MetaCodeGeneratorViewModel : Base.ViewModel
    {
        private readonly MetaSearchService _metaSearchService;

        public MetaCodeGeneratorViewModel()
        {
            _metaSearchService = new MetaSearchService();
            ConvertCommand = new RelayCommand(OnConvertExecuted, CanConvertExecute);
        }

        #region Исходный текст
        private string _sourceText = string.Empty;
        public string SourceText
        {
            get => _sourceText;
            set => Set(ref _sourceText, value);
        }
        #endregion

        #region Итоговый текст
        private string _resultText = string.Empty;
        public string ResultText
        {
            get => _resultText;
            set => Set(ref _resultText, value);
        }
        #endregion

        #region Команда конвертации
        public ICommand ConvertCommand { get; set; }

        private void OnConvertExecuted(object obj)
        {
            ResultText = _metaSearchService.Convert(SourceText);
        }

        private bool CanConvertExecute(object obj)
        {
            return !string.IsNullOrEmpty(SourceText);
        }
        #endregion
    }
}
