using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PilotObjectInfo.Behaviors.CopyBehaviors
{
    public class TypesMainCopyCellValueBehavior : CopyCellValueBehavior
    {
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
                var content = currentCell.Column.GetCellContent(currentCell.Item) as TextBlock;
                Clipboard.SetText(content.Text);

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
