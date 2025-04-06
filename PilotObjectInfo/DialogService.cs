using System;
using System.Linq;
using Ascon.Pilot.SDK;
using PilotObjectInfo.Extensions;
using PilotObjectInfo.ViewModels;
using PilotObjectInfo.Views;

namespace PilotObjectInfo
{
    class DialogService
    {
        static public void ShowInfo(IDataObject obj, IObjectsRepository objectsRepository,  IFileProvider fileProvider,ITabServiceProvider tabServiceProvider, FileModifier fileModifier )
        {
            if (obj == null) return;
            var vm = new MainViewModel(obj, objectsRepository, fileModifier, fileProvider, tabServiceProvider);
            var v = new MainView() { DataContext = vm };
            v.Show();

        }

        static public async void ShowInfo(Guid id, IObjectsRepository objectsRepository, IFileProvider fileProvider, ITabServiceProvider tabServiceProvider, FileModifier fileModifier)
        {
            var obj = (await objectsRepository.GetObjectsAsync(new Guid[] { id }, o => o, System.Threading.CancellationToken.None)).FirstOrDefault();
            ShowInfo(obj, objectsRepository, fileProvider, tabServiceProvider, fileModifier);

        }
    }
}
