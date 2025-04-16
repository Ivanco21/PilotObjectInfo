using System.ComponentModel.Composition;
using System.Linq;
using Ascon.Pilot.SDK;
using Ascon.Pilot.SDK.Menu;

namespace PilotObjectInfo
{
    [Export(typeof(IMenu<StorageContext>))]
    [Export(typeof(IMenu<ObjectsViewContext>))]
    [Export(typeof(IMenu<TasksViewContext2>))]
    public class ObjectMenu : IMenu<ObjectsViewContext>, IMenu<StorageContext>, IMenu<TasksViewContext2>
    {
        private IObjectsRepository _objectsRepository;
        private ISearchService _searchService;
        private IObjectModifier _objectModifier;
        private IFileProvider _fileProvider;
        private ITabServiceProvider _tabServiceProvider;
        private FileModifier _fileModifier;

        [ImportingConstructor]
        public ObjectMenu(IObjectsRepository objectsRepository, ISearchService searchService, IFileProvider fileProvider, ITabServiceProvider tabServiceProvider, IObjectModifier objectModifier)
        {
            _objectsRepository = objectsRepository;
            _searchService = searchService;
            _objectModifier = objectModifier;
            _fileProvider = fileProvider;
            _tabServiceProvider = tabServiceProvider;
            _fileModifier = new FileModifier(_objectModifier, _objectsRepository);
        }

        public void Build(IMenuBuilder builder, StorageContext context)
        {
            AddItem(builder);
        }

        public void Build(IMenuBuilder builder, ObjectsViewContext context)
        {
            AddItem(builder);
        }

        public void Build(IMenuBuilder builder, TasksViewContext2 context)
        {
            AddItem(builder);
        }

        public void OnMenuItemClick(string name, StorageContext context)
        {
            if (context.SelectedObjects.Count() > 1 || context.SelectedObjects.Count() == 0) return;
            IDataObject obj = context.SelectedObjects.First().DataObject;
            DialogService.ShowInfo(obj, _objectsRepository, _searchService, _fileProvider, _tabServiceProvider, _fileModifier);
        }

        public void OnMenuItemClick(string name, ObjectsViewContext context)
        {
            if (context.SelectedObjects.Count() > 1 || context.SelectedObjects.Count() == 0) return;
            IDataObject obj = context.SelectedObjects.First();
            DialogService.ShowInfo(obj, _objectsRepository, _searchService, _fileProvider, _tabServiceProvider, _fileModifier);
        }

        public void OnMenuItemClick(string name, TasksViewContext2 context)
        {
            if (context.SelectedTasks.Count() > 1 || context.SelectedTasks.Count() == 0) return;
            IDataObject obj = context.SelectedTasks.First();
            DialogService.ShowInfo(obj, _objectsRepository, _searchService, _fileProvider, _tabServiceProvider, _fileModifier);
        }

        private void AddItem(IMenuBuilder builder)
        {
            builder.AddItem("objectInfo", 0).WithHeader("Информация об объекте");
        }
    }
}
