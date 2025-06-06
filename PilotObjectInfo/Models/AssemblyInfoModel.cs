
using System.Collections.Generic;

namespace PilotObjectInfo.Models
{
    internal class AssemblyInfoModel
    {
        public AssemblyInfoModel()
        {
        }

        public string InternalName { get; set; }
        public string Company { get; set; }
        public string FullPath { get; set; }
        public string Version { get; set; }
        public ExtensionType ExtensionType { get; set; }
        internal HashSet<TypeModel> Types { get; set; } = new();
        internal HashSet<AttributeModel> Attributes { get; set; } = new();
    }
}
