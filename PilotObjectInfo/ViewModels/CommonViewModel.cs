using PilotObjectInfo.Settings;
using PilotObjectInfo.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace PilotObjectInfo.ViewModels
{
    class CommonViewModel : Base.ViewModel
    {
        protected CommonViewModel()
        {
            #region Commands
            CopyRowCommand = new RelayCommand(OnCopyRowExecuted, CanCopyRowExecute);
            CopyCsvCommand = new RelayCommand(OnCopyCsvExecuted, CanCopyCsvExecute);
            CopyJsonCommand = new RelayCommand(OnCopyJsonExecuted, CanCopyJsonExecute);
            CopyCellCommand = new RelayCommand(OnCopyCellExecuted);
            #endregion
        }

        #region CopyCellCommand
        public ICommand CopyCellCommand { get; }

        private void OnCopyCellExecuted(object parameter)
        {
            if (parameter is not MouseButtonEventArgs e ||
                e.OriginalSource is not DependencyObject source)
                return;

            var cell = FindAncestor<DataGridCell>(source);
            if (cell?.Column == null)
                return;

            var text = ReadCell(cell.Column, cell.DataContext);
            if (string.IsNullOrEmpty(text))
                return;

            Clipboard.SetText(text);
            ShowCopiedPopup(e, cell);

            e.Handled = true;
        }

        private int _copiedPopupVersion;
        private bool _isCopiedPopupOpen;
        public bool IsCopiedPopupOpen
        {
            get => _isCopiedPopupOpen;
            set => Set(ref _isCopiedPopupOpen, value);
        }

        private double _copiedPopupX;
        public double CopiedPopupX
        {
            get => _copiedPopupX;
            set => Set(ref _copiedPopupX, value);
        }

        private double _copiedPopupY;
        public double CopiedPopupY
        {
            get => _copiedPopupY;
            set => Set(ref _copiedPopupY, value);
        }
        #endregion

        #region CopyRowCommand
        public ICommand CopyRowCommand { get; }

        private void OnCopyRowExecuted(object parameter)
        {
            if (parameter is not DataGrid grid ||
                !IsDataItem(grid.SelectedItem))
                return;

            var columns = VisibleColumns(grid);
            if (columns.Count == 0)
                return;

            var text = string.Join(" ", columns.Select(column => ReadCell(column, grid.SelectedItem)));
            Clipboard.SetText(text);
        }

        private bool CanCopyRowExecute(object parameter)
        {
            return parameter is DataGrid grid && IsDataItem(grid.SelectedItem);
        }
        #endregion

        #region CopyCsvCommand
        public ICommand CopyCsvCommand { get; }

        private void OnCopyCsvExecuted(object parameter)
        {
            if (parameter is not DataGrid grid)
                return;

            var columns = VisibleColumns(grid);
            if (columns.Count == 0)
                return;

            var builder = new StringBuilder();
            AppendCsvRow(builder, columns.Select(column => column.Header?.ToString()));

            foreach (var item in grid.Items)
            {
                if (!IsDataItem(item))
                    continue;

                AppendCsvRow(builder, columns.Select(column => ReadCell(column, item)));
            }

            Clipboard.SetText(builder.ToString());
        }

        private bool CanCopyCsvExecute(object parameter)
        {
            return parameter is DataGrid grid && HasDataItems(grid);
        }
        #endregion

        #region CopyJsonCommand
        public ICommand CopyJsonCommand { get; }

        private void OnCopyJsonExecuted(object parameter)
        {
            if (parameter is not DataGrid grid)
                return;

            var columns = VisibleColumns(grid);
            if (columns.Count == 0)
                return;

            var builder = new StringBuilder();
            builder.Append('[');
            var firstRow = true;
            foreach (var item in grid.Items)
            {
                if (!IsDataItem(item))
                    continue;

                if (!firstRow)
                    builder.Append(',');
                firstRow = false;

                builder.Append('{');
                for (var i = 0; i < columns.Count; i++)
                {
                    if (i > 0)
                        builder.Append(',');

                    builder.Append('"');
                    builder.Append(EscapeJson(columns[i].Header?.ToString()));
                    builder.Append("\":\"");
                    builder.Append(EscapeJson(ReadCell(columns[i], item)));
                    builder.Append('"');
                }
                builder.Append('}');
            }
            builder.Append(']');

            Clipboard.SetText(builder.ToString());
        }

        private bool CanCopyJsonExecute(object parameter)
        {
            return parameter is DataGrid grid && HasDataItems(grid);
        }
        #endregion

        private static bool HasDataItems(DataGrid grid)
        {
            foreach (var item in grid.Items)
            {
                if (IsDataItem(item))
                    return true;
            }

            return false;
        }

        private static List<DataGridColumn> VisibleColumns(DataGrid grid)
        {
            return grid.Columns
                .Where(column => column.Visibility == Visibility.Visible)
                .OrderBy(column => column.DisplayIndex)
                .ToList();
        }

        private static bool IsDataItem(object item)
        {
            return item != null && !ReferenceEquals(item, CollectionView.NewItemPlaceholder);
        }

        private static string EscapeJson(string value)
        {
            return (value ?? string.Empty)
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\r", "\\r")
                .Replace("\n", "\\n");
        }

        private static void AppendCsvRow(StringBuilder builder, IEnumerable<string> fields)
        {
            var index = 0;
            foreach (var field in fields)
            {
                if (index > 0)
                    builder.Append(AppSettings.CSV_DIVIDER);

                builder.Append(field);
                index++;
            }

            builder.AppendLine();
        }

        private void ShowCopiedPopup(MouseButtonEventArgs e, DataGridCell cell)
        {
            var host = FindAncestor<UserControl>(cell) as IInputElement
                       ?? FindAncestor<DataGrid>(cell);
            if (host == null)
                return;

            var position = e.GetPosition(host);
            CopiedPopupX = position.X + 8;
            CopiedPopupY = position.Y - 28;

            var version = ++_copiedPopupVersion;
            IsCopiedPopupOpen = false;
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                if (version == _copiedPopupVersion)
                    IsCopiedPopupOpen = true;
            }), DispatcherPriority.Input);

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += (_, _) =>
            {
                timer.Stop();
                if (version == _copiedPopupVersion)
                    IsCopiedPopupOpen = false;
            };
            timer.Start();
        }

        private static T FindAncestor<T>(DependencyObject current)
            where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T match)
                    return match;

                if (current is Visual or System.Windows.Media.Media3D.Visual3D)
                    current = VisualTreeHelper.GetParent(current);
                else if (current is ContentElement content)
                    current = ContentOperations.GetParent(content);
                else
                    current = LogicalTreeHelper.GetParent(current);
            }

            return null;
        }

        private static string ReadCell(DataGridColumn column, object item)
        {
            if (item == null ||
                column is not DataGridBoundColumn bound ||
                bound.Binding is not Binding binding)
                return string.Empty;

            var path = binding.Path?.Path;

            if (string.IsNullOrEmpty(path))
                return item.ToString();

            return item.GetType().GetProperty(path)?.GetValue(item)?.ToString() ?? string.Empty;
        }
    }
}
