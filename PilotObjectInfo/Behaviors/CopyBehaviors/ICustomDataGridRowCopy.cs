using System.Windows.Input;

namespace PilotObjectInfo.Behaviors.CopyBehaviors
{
    interface ICustomDataGridRowCopy
    {
        void OnPreviewKeyDownCustom(object sender, KeyEventArgs e);
    }
}
