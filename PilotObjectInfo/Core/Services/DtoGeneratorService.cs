
using System.Text;
using Ascon.Pilot.SDK;
using PilotObjectInfo.Core.Extensions;

namespace PilotObjectInfo.Core.Services
{
    internal class DtoGeneratorService
    {
        internal StringBuilder GenerateFullDTO(IType type)
        {
            StringBuilder sb = new();
            string dName = $"{type.Name.SnakeToPascalCase()}DTO";

            sb.AppendLine($"internal class {dName} : ObjectDTO");
            sb.AppendLine($"{{");
            sb.AppendLine($"    /// <summary>");
            sb.AppendLine($"    /// {type.Title}");
            sb.AppendLine($"    /// </summary>");
            sb.AppendLine($"    internal const string TYPE_NAME = \"{type.Name}\";");
            sb.AppendLine();
            sb.AppendLine($"    internal {dName}() {{ }}");
            sb.AppendLine();
            sb.AppendLine($"    internal {dName}(IDataObject nObj)");
            sb.AppendLine($"    {{");
            sb.AppendLine($"        Id = nObj.Id;");
            sb.AppendLine($"        Attrs = new AttrManager(nObj);");
            sb.AppendLine($"        ConvertToDTO();");
            sb.AppendLine($"    }}");
            sb.AppendLine();

            foreach (IAttribute attr in type.Attributes)
            {
                sb.AppendLine($"    internal {attr.Type.ToString().ToLower()} {attr.Name.SnakeToPascalCase()} {{ get; set; }}");
            }
            sb.AppendLine();

            sb.AppendLine($"    protected override void ConvertToDTO()");
            sb.AppendLine($"    {{");
            foreach (IAttribute attr in type.Attributes)
            {
                sb.AppendLine($"        {attr.Name.SnakeToPascalCase()} = Attrs.GetAttributeStrOrEmpty({attr.Name.ToUpper()});");
            }
            sb.AppendLine($"    }}");
            sb.AppendLine();

            foreach (IAttribute attr in type.Attributes)
            {
                sb.AppendLine($"    /// <summary>");
                sb.AppendLine($"    /// {attr.Title}");
                sb.AppendLine($"    /// </summary>");
                sb.AppendLine($"    internal const {attr.Type.ToString().ToLower()} {attr.Name.ToUpper()} = \"{attr.Name}\";");
                sb.AppendLine();
            }
            sb.AppendLine($"}}");

            return sb;
        }

    }
}
