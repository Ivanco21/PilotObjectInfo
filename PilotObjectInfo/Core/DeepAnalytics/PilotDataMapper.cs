using System.Collections.Generic;
using System.Linq;
using PilotObjectInfo.Models;


namespace PilotObjectInfo.Core.DeepAnalytics
{
    internal class PilotDataMapper
    {
        internal HashSet<TypeModel> TypesPilot { get; set; } = new();
        internal HashSet<AttributeModel> AttributesPilot { get; set; } = new();
        internal HashSet<UserStateModel> UserStatesPilot { get; set; } = new();

        internal AssemblyInfoModel GetAssemblyInfoModel(AssemblyRawData assemblyRaw)
        {
            AssemblyInfoModel assemblyInfo = new()
            {
                InternalName = assemblyRaw.Metadata?.InternalName,
                Product = assemblyRaw.Metadata?.Product,
                Company = assemblyRaw.Metadata?.Company,
                Version = assemblyRaw.Metadata?.Version,
            };

            assemblyInfo.Types = TypesPilot.AsParallel()
                                            .Where(t => 
                                                assemblyRaw.Strings.Contains(t.Name) || 
                                                assemblyRaw.Strings.Contains(t.Title))
                                            .ToHashSet();

            assemblyInfo.Attributes = AttributesPilot.AsParallel().Where(a => assemblyRaw.Strings.Contains(a.Name)).ToHashSet();

            assemblyInfo.UserStates = UserStatesPilot.AsParallel()
                                                    .Where(a => 
                                                            assemblyRaw.Strings.Contains(a.Id.ToString()) || 
                                                            assemblyRaw.Strings.Contains(a.Name) || 
                                                            assemblyRaw.Strings.Contains(a.Title))
                                                    .ToHashSet();

            return assemblyInfo;
        }
    }
}
