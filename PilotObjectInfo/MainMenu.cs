﻿using System;
using System.ComponentModel.Composition;
using System.Text.RegularExpressions;
using System.Windows;
using Ascon.Pilot.SDK;
using Ascon.Pilot.SDK.Menu;

namespace PilotObjectInfo
{

    [Export(typeof(IMenu<MainViewContext>))]
    public class MainMenu : IMenu<MainViewContext>
    {
        
        IObjectsRepository _objectsRepository;
        private IFileProvider _fileProvider;
        private ITabServiceProvider _tabServiceProvider;
        private IObjectModifier _objectModifier;
        private FileModifier _fileModifier;
        private const string SHOW_SUB_MENU = "ShowObjectInfo";
        private const string GO_SUB_MENU = "GoToObject";
        private const string GUID_PATTERN = @"([0-9A-F]{8}[-]?(?:[0-9A-F]{4}[-]?){3}[0-9A-F]{12})";

        [ImportingConstructor]
        public MainMenu(IObjectsRepository objectsRepository, IFileProvider fileProvider, ITabServiceProvider tabServiceProvider, IObjectModifier objectModifier)
        {
            _objectsRepository = objectsRepository;
            _fileProvider = fileProvider;
            _tabServiceProvider = tabServiceProvider;
            _objectModifier = objectModifier;
            _fileModifier = new FileModifier(_objectModifier, _objectsRepository);

        }

        public void Build(IMenuBuilder builder, MainViewContext context)
        {
            var item = builder.AddItem("ObjectInfo", 1).WithHeader("Object Info");
            item.WithSubmenu().AddItem(SHOW_SUB_MENU, 0).WithHeader("Show");
            item.WithSubmenu().AddItem(GO_SUB_MENU, 0).WithHeader("Go");
        }

        public void OnMenuItemClick(string name, MainViewContext context)
        {
            if (name != SHOW_SUB_MENU && name != GO_SUB_MENU) return;
            
            Regex rgx = new Regex(GUID_PATTERN, RegexOptions.IgnoreCase);
            var clipboardText = Clipboard.GetText(TextDataFormat.Text);
            var match = rgx.Match(clipboardText);
            if (match.Success == false) return;
            var id = new Guid(match.Groups[1].Value);

            if (name == SHOW_SUB_MENU)
                DialogService.ShowInfo(id, _objectsRepository, _fileProvider, _tabServiceProvider, _fileModifier);
            if (name == GO_SUB_MENU)
                _tabServiceProvider.ShowElement(id);

        }
    }
}
