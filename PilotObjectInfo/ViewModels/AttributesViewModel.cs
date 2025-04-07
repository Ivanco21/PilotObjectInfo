using System;
using System.Collections.ObjectModel;
using System.Linq;
using Ascon.Pilot.SDK;
using PilotObjectInfo.Models;

namespace PilotObjectInfo.ViewModels
{
    class AttributesViewModel : Base.ViewModel
    {
        public AttributesViewModel(IDataObject obj)
        {
            _attributes = new ObservableCollection<AttributeModel>();

            foreach (var attr in obj.Attributes)
            {
                var attrType = obj.Type.Attributes.FirstOrDefault(x => x.Name.Equals(attr.Key));
                if (attrType == null)
                {
                    _attributes.Add( new AttributeModel(attr.Key, attr.Value?.ToString(), attrType.Title));
                    continue;
                }
                switch (attrType.Type)
                {
                    case AttributeType.Array:
                    case AttributeType.OrgUnit:
                        _attributes.Add(new AttributeModel(attr.Key, (ArrayToString<int>(attr.Value)), attrType.Title));
                        break;
                    case AttributeType.Integer:
                    case AttributeType.Double:
                    case AttributeType.DateTime:
                    case AttributeType.String:
                    case AttributeType.Decimal:
                    case AttributeType.Numerator:
                    case AttributeType.UserState:
                    default:
                        _attributes.Add(new AttributeModel(attr.Key, attr.Value?.ToString(), attrType.Title));
                        break;
                }
            }
        }

        private string ArrayToString<T>(object value)
        {
            if (!(value is T[] arr)) return value?.ToString();
            return $"[{String.Join(",", arr)}]";
        }

        private ObservableCollection<AttributeModel> _attributes;
        public ObservableCollection<AttributeModel> Attributes
        {
            get => _attributes;
        }
    }
}
