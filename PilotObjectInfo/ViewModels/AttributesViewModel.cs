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
                AttributeModel attModel = new();
                attModel.Name = attr.Key;
                attModel.Type = GetAttrType(obj, attr.Key);

               var attrType = obj.Type.Attributes.FirstOrDefault(x => x.Name.Equals(attr.Key));
                if (attrType == null)
                {
                    attModel.Value = "атрибута нет в типе!";
                    attModel.Title = string.Empty;
                    continue;
                }
                switch (attrType.Type)
                {
                    case AttributeType.Array:
                    case AttributeType.OrgUnit:
                        attModel.Value = (ArrayToString<int>(attr.Value));
                        attModel.Title = attrType.Title;
                        break;
                    case AttributeType.Integer:
                    case AttributeType.Double:
                    case AttributeType.DateTime:
                    case AttributeType.String:
                    case AttributeType.Decimal:
                    case AttributeType.Numerator:
                    case AttributeType.UserState:
                    default:
                        attModel.Value = attr.Value?.ToString();
                        attModel.Title = attrType.Title;
                        break;
                }

                _attributes.Add(attModel);
            }

            foreach (var attribute in obj.Type.Attributes)
            {
                if (! _attributes.Any(a => a.Name == attribute.Name))
                {
                    AttributeModel attModel = new()
                    {
                        Name = attribute.Name,
                        Value = "атрибут объекта не задан",
                        Title = attribute.Title,
                        Type = GetAttrType(obj, attribute.Name)
                    };

                    _attributes.Add(attModel);
                }
            }
        }

        private string GetAttrType(IDataObject obj, string attrName)
        {
            
            if (obj.Type.Attributes.Any(a => a.Name == attrName))
            {
                return obj.Type.Attributes.First(a => a.Name == attrName).Type.ToString();
            }
            else
            {
                return string.Empty;
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
