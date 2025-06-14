
using Ascon.Pilot.SDK;

namespace PilotObjectInfo.Models
{
    public record AttributeModel
    {
        public AttributeModel()
        {
        }

        public AttributeModel(string name, string value, string title)
        {
            Name = name;
            Value = value;
            Title = title;
        }

        public AttributeModel(IAttribute attribute)
        {
            Name = attribute.Name;
            Title = attribute.Title;
            Type = attribute.Type.ToString();
            Configuration2 = attribute.Configuration2();
        }

        public string Name { get; set; }
        public string Value { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Configuration2 { get; set; }

    }
}
