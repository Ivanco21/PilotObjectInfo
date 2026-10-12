using Ascon.Pilot.SDK;
using PilotObjectInfo.Models;
using System.Collections.Generic;

namespace PilotObjectInfo.ViewModels
{
    class TypeViewModel : CommonViewModel
    {
        public TypeViewModel(IType type)
        {
            Properties = new List<NameValueModel>
            {
                new(nameof(type.Id), type.Id.ToString()),
                new(nameof(type.Name), type.Name),
                new(nameof(type.Title), type.Title),
                new(nameof(type.Kind), type.Kind.ToString()),
                new(nameof(type.IsMountable), type.IsMountable.ToString()),
                new(nameof(type.IsProject), type.IsProject.ToString()),
                new(nameof(type.IsService), type.IsService.ToString()),
            };
        }

        public List<NameValueModel> Properties { get; }
    }
}
