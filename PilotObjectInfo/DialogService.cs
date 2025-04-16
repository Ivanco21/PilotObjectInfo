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
        //TODO: все интерфейсы от Pilot через Global использовать, чтобы везде не инжектить
        static public void ShowInfo(IDataObject obj, IObjectsRepository objectsRepository, ISearchService searchService,  IFileProvider fileProvider,ITabServiceProvider tabServiceProvider, FileModifier fileModifier )
        {
            if (obj == null) return;
            var vm = new MainViewModel(obj, objectsRepository, searchService, fileModifier, fileProvider, tabServiceProvider);
            var v = new MainView() { DataContext = vm };
            v.Show();

            InitGlobalInterfaces(searchService, tabServiceProvider);
        }

        static public async void ShowInfo(Guid id, IObjectsRepository objectsRepository, ISearchService searchService, IFileProvider fileProvider, ITabServiceProvider tabServiceProvider, FileModifier fileModifier)
        {
            var obj = (await objectsRepository.GetObjectsAsync(new Guid[] { id }, o => o, System.Threading.CancellationToken.None)).FirstOrDefault();
            ShowInfo(obj, objectsRepository, searchService, fileProvider, tabServiceProvider, fileModifier);
        }

        static void InitGlobalInterfaces(ISearchService searchService, ITabServiceProvider tabServiceProvider)
        {
            GI.SearchService = searchService;
            GI.TabServiceProvider = tabServiceProvider;
        }
    }
}
