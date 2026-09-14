namespace PilotObjectInfo.Dto;

internal class MinMetaDto
{
    public string Name { get; set; }
    public string Title { get; set; }
    public MetaClass MetaClass { get; set; }
    
    internal MinMetaDto(string name, string title, MetaClass metaClass)
    {
        Name = name;
        Title = title;
        MetaClass = metaClass;
    }
}