using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace PilotObjectInfo.Behaviors
{
    public class CopyCellValueBehavior : Behavior<DataGrid>
    {
        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register("Key", typeof(Key), typeof(CopyCellValueBehavior),
            new PropertyMetadata(Key.None));

        public Key Key
        {
            get => (Key)GetValue(KeyProperty);
            set => SetValue(KeyProperty, value);
        }
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewKeyDown += OnPreviewKeyDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewKeyDown -= OnPreviewKeyDown;
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
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
        }
    }
}
