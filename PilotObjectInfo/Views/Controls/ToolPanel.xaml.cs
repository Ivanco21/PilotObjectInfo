using System.Windows;
using System.Windows.Controls;

namespace PilotObjectInfo.Views.Controls
{
    /// <summary>
    /// Interaction logic for ToolPanel.xaml
    /// </summary>
    public partial class ToolPanel : UserControl
    {
        public static readonly DependencyProperty ToolsProperty =
            DependencyProperty.Register(
                nameof(Tools),
                typeof(object),
                typeof(ToolPanel),
                new PropertyMetadata(null));

        public ToolPanel()
        {
            InitializeComponent();
        }

        public object Tools
        {
            get => GetValue(ToolsProperty);
            set => SetValue(ToolsProperty, value);
        }
    }
}
