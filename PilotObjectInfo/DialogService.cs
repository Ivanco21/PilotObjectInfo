using System;
using System.Linq;
using Ascon.Pilot.SDK;
using PilotObjectInfo.Core;
using PilotObjectInfo.Extensions;
using PilotObjectInfo.ViewModels;
using PilotObjectInfo.Views;

namespace PilotObjectInfo
{
    class DialogService
    {
        static public void ShowInfo(IDataObject obj, FileModifier fileModifier)
        {
            if (obj == null) return;
            var vm = new MainViewModel(obj, fileModifier);
            var v = new MainView() { DataContext = vm };
            v.Show();

        }

        static public async void ShowInfo(Guid id, FileModifier fileModifier)
        {
            var obj = (await GI.Repository.GetObjectsAsync(new Guid[] { id }, o => o, System.Threading.CancellationToken.None)).FirstOrDefault();
            ShowInfo(obj, fileModifier);
        }
    }
}
