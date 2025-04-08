
namespace PilotObjectInfo.Models
{
    public class AttributeModel
    {
        public AttributeModel(string name, string value, string title)
        {
            Name = name;
            Value = value;
            Title = title;
        }

        public string Name { get; set; }
        public string Value { get; set; }
        public string Title { get; set; }
    }
}
