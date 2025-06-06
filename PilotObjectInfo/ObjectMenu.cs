using Ascon.Pilot.SDK;
using Ascon.Pilot.SDK.Menu;
using PilotObjectInfo.Core;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace PilotObjectInfo
{
    [Export(typeof(IMenu<StorageContext>))]
    [Export(typeof(IMenu<ObjectsViewContext>))]
    [Export(typeof(IMenu<TasksViewContext2>))]
    public class ObjectMenu : IMenu<ObjectsViewContext>, IMenu<StorageContext>, IMenu<TasksViewContext2>
    {
        private FileModifier _fileModifier;

        [ImportingConstructor]
        public ObjectMenu(IObjectsRepository objectsRepository, 
                        ISearchService searchService, 
                        IFileProvider fileProvider, 
                        ITabServiceProvider tabServiceProvider, 
                        IObjectModifier objectModifier)
        {
            GI.Repository = objectsRepository;
            GI.SearchService = searchService;
            GI.Modifier = objectModifier;
            GI.FileProvider = fileProvider;
            GI.TabServiceProvider = tabServiceProvider;
            _fileModifier = new();
        }

        void IMenu<StorageContext>.Build(IMenuBuilder builder, StorageContext context)
        {
            AddMenuItem(builder);
        }

        void IMenu<ObjectsViewContext>.Build(IMenuBuilder builder, ObjectsViewContext context)
        {
            AddMenuItem(builder);
        }

        void IMenu<TasksViewContext2>.Build(IMenuBuilder builder, TasksViewContext2 context)
        {
            AddMenuItem(builder);
        }

        void IMenu<StorageContext>.OnMenuItemClick(string name, StorageContext context)
        {
            if (context.SelectedObjects.Count() > 1 || context.SelectedObjects.Count() == 0) return;
            IDataObject obj = context.SelectedObjects.First().DataObject;
            DialogService.ShowInfo(obj, _fileModifier);
        }

        void IMenu<ObjectsViewContext>.OnMenuItemClick(string name, ObjectsViewContext context)
        {
            if (context.SelectedObjects.Count() > 1 || context.SelectedObjects.Count() == 0) return;
            IDataObject obj = context.SelectedObjects.First();
            DialogService.ShowInfo(obj, _fileModifier);
        }

        void IMenu<TasksViewContext2>.OnMenuItemClick(string name, TasksViewContext2 context)
        {
            if (context.SelectedTasks.Count() > 1 || context.SelectedTasks.Count() == 0)
                return;
            IDataObject obj = context.SelectedTasks.First();
            DialogService.ShowInfo(obj, _fileModifier);
        }

        private void AddMenuItem(IMenuBuilder builder)
        {
            builder.AddItem("objectInfo", 0).WithHeader("Информация об объекте");
        }
    }
}
