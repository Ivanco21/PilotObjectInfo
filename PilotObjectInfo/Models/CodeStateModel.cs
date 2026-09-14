using System;
using System.Collections.Generic;

namespace PilotObjectInfo.Models
{
    public class CodeStateModel
    {
        public string Path { get; set; }
        public Guid Id { get; set; }
        public string InternalName { get; set; }
        public string Version { get; set; }
        public string Product { get; set; }
        public string Company { get; set; }

        public List<string> CodeParts { get; set; } = new();
    }
}
