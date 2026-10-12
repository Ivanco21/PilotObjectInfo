namespace PilotObjectInfo.Models
{
    public record NameValueModel
    {
        public NameValueModel()
        {
        }

        public NameValueModel(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public string Value { get; set; }
    }
}
