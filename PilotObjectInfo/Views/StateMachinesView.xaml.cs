using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GraphShape.Controls;
using PilotObjectInfo.Models;
using PilotObjectInfo.ViewModels;

namespace PilotObjectInfo.Views
{
    public partial class StateMachinesView : UserControl
    {
        public StateMachinesView()
        {
            InitializeComponent();
        }

        private void OnVertexMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is not VertexControl vertexControl)
                return;

            if (vertexControl.Vertex is not StateMachineVertex vertex)
                return;

            if (DataContext is not StateMachinesViewModel viewModel)
                return;

            if (viewModel.SelectUserStateCommand.CanExecute(vertex))
                viewModel.SelectUserStateCommand.Execute(vertex);

            e.Handled = true;
        }

        private void OnGraphMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Clear selection when clicking empty graph area (not a vertex/edge).
            if (e.OriginalSource is VertexControl || e.OriginalSource is EdgeControl)
                return;

            if (FindAncestor<VertexControl>(e.OriginalSource as DependencyObject) != null)
                return;

            if (FindAncestor<EdgeControl>(e.OriginalSource as DependencyObject) != null)
                return;

            if (DataContext is not StateMachinesViewModel viewModel)
                return;

            viewModel.ClearUserStateSelection();
        }

        private static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T match)
                    return match;
                current = VisualTreeHelper.GetParent(current);
            }

            return null;
        }
    }
}
