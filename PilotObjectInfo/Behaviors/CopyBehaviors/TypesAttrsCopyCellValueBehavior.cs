using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PilotObjectInfo.Behaviors.CopyBehaviors
{
    public class TypesAttrsCopyCellValueBehavior : CopyCellValueBehavior
    {
        private static string GetCellDisplayText(FrameworkElement cellRoot)
        {
            if (cellRoot is TextBlock direct)
            {
                return direct.Text;
            }

            if (cellRoot is ContentPresenter presenter)
            {
                if (presenter.Content is TextBlock fromContent)
                {
                    return fromContent.Text;
                }

                var inTemplate = FindVisualChild<TextBlock>(presenter);
                if (inTemplate != null)
                {
                    return inTemplate.Text;
                }
            }

            var nested = FindVisualChild<TextBlock>(cellRoot);
            return nested?.Text ?? string.Empty;
        }

        private static T FindVisualChild<T>(DependencyObject parent)
            where T : DependencyObject
        {
            var count = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T match)
                {
                    return match;
                }

                var nested = FindVisualChild<T>(child);
                if (nested != null)
                {
                    return nested;
                }
            }

            return null;
        }

        public override void OnPreviewKeyDownCustom(object sender, KeyEventArgs e)
        {
            if (sender is not DataGrid)
            {
                return;
            }

            var dataGrid = sender as DataGrid;
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.C)
            {
                var currentCell = dataGrid.CurrentCell;
                var content = currentCell.Column?.GetCellContent(currentCell.Item);
                if (content != null)
                {
                    var text = GetCellDisplayText(content);
                    if (text.Length > 0)
                    {
                        Clipboard.SetText(text);
                    }
                }

                e.Handled = true;
            }

            if ((Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) ==
                (ModifierKeys.Control | ModifierKeys.Shift) &&
                e.Key == Key.C)
            {
                var row = dataGrid.SelectedItem;
                var name = row.GetType().GetProperty("Name").GetValue(row);
                var title = row.GetType().GetProperty("Title").GetValue(row);
                Clipboard.SetText($"{title} ({name})");

                e.Handled = true;
            }

            if ((Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Alt)) ==
                (ModifierKeys.Control | ModifierKeys.Alt) &&
                e.Key == Key.C)
            {
                var row = dataGrid.SelectedItem;
                var name = row.GetType().GetProperty("Name").GetValue(row);
                var title = row.GetType().GetProperty("Title").GetValue(row);

                Clipboard.SetText($"{name} ({title})");

                e.Handled = true;
            }
        }
    }
}
