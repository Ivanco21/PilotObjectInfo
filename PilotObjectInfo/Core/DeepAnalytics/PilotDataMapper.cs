using System.Collections.Generic;
using System.Linq;
using PilotObjectInfo.Models;


namespace PilotObjectInfo.Core.DeepAnalytics
{
    internal class PilotDataMapper
    {
        internal HashSet<TypeModel> PilotTypes { get; set; } = new();
        internal HashSet<AttributeModel> PilotAttributes { get; set; } = new();

        internal AssemblyInfoModel GetAssemblyInfoModel(AssemblyRawData assemblyRaw)
        {
            AssemblyInfoModel assemblyInfo = new()
            {
                Company = assemblyRaw.Metadata?.Company,
                InternalName = assemblyRaw.Metadata?.InternalName,
                Version = assemblyRaw.Metadata?.Version,
            };

            assemblyInfo.Types = PilotTypes.AsParallel().Where(t => assemblyRaw.Strings.Contains(t.Name)).ToHashSet();
            assemblyInfo.Attributes = PilotAttributes.AsParallel().Where(a => assemblyRaw.Strings.Contains(a.Name)).ToHashSet();

            return assemblyInfo;
        }
    }
}
