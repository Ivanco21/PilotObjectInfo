
using Ascon.Pilot.SDK;

namespace PilotObjectInfo.Models
{
    public class AttributeDescriptionModel
    {
        public AttributeDescriptionModel()
        {
        }

        public AttributeDescriptionModel(IAttribute attribute)
        {
            Name = attribute.Name;
            Title = attribute.Title;
            Type = attribute.Type.ToString();
        }

        public string Name { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }

    }
}
