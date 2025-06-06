﻿using Ascon.Pilot.SDK;

namespace PilotObjectInfo.ViewModels
{
    class TypeViewModel : Base.ViewModel
    {
		private IType _type;

		public TypeViewModel(IType type)
		{
			_type = type;
		}
		public int Id => _type.Id;
		public string Name => _type.Name;
		public string Title => _type.Title;
		public string Kind => _type.Kind.ToString();
		public bool IsMountable => _type.IsMountable;
		public bool IsProject => _type.IsProject;
		public bool IsService => _type.IsService;
		
	}
}
