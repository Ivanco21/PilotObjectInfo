using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace PilotObjectInfo.Behaviors.CopyBehaviors
{
    abstract public class CopyCellValueBehavior : Behavior<DataGrid>, ICustomDataGridRowCopy
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
            this.OnPreviewKeyDownCustom(sender, e);
        }

        public abstract void OnPreviewKeyDownCustom(object sender, KeyEventArgs e);
    }
}
