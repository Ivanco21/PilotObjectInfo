using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;

namespace PilotObjectInfo.Core.DeepAnalytics
{
    internal partial class AssemblyParser
    {

        private const string EXT_TARGET = ".ext2.dll";
        const string _searchPattern = @"([""])(?:(?=(\\?))\2.)*?\1";

        internal List<string> GetLoadedDllPaths()
        {

            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                                                    .Where(a => !a.IsDynamic)
                                                    .Where(a => a.Location.EndsWith(EXT_TARGET, StringComparison.OrdinalIgnoreCase));
            return assemblies.Select(a => a.Location).ToList();
        }

        internal AssemblyRawData DoParse(string path)
        {
            AssemblyRawData rawData = new();

            using (FileStream peFileStream = new(path, FileMode.Open, FileAccess.Read))
            using (PEFile peFile = new(path, peFileStream))
            using (StringWriter writer = new())
            {
                PlainTextOutput output = new(writer);

                var metaReader = peFile.Metadata;
                AssemblyDefinition assemblyDefinition = peFile.Metadata.GetAssemblyDefinition();

                rawData.Metadata.Id = metaReader.GetGuid(metaReader.GetModuleDefinition().Mvid);
                rawData.Metadata.InternalName = assemblyDefinition.GetAssemblyName().Name;
                rawData.Metadata.Version = assemblyDefinition.GetAssemblyName().Version.ToString();

                foreach (var attributeHandle in assemblyDefinition.GetCustomAttributes())
                {
                    var attribute = metaReader.GetCustomAttribute(attributeHandle);
                    var attrType = attribute.GetAttributeType(metaReader);

                    if (attrType.GetFullTypeName(metaReader).Name == "AssemblyCompanyAttribute")
                    {
                        var companyValue = metaReader.GetBlobBytes(attribute.Value);
                        rawData.Metadata.Company = System.Text.Encoding.UTF8.GetString(companyValue, 3, companyValue.Length - 3).TrimEnd('\0');
                    }

                    if (attrType.GetFullTypeName(metaReader).Name == "AssemblyProductAttribute")
                    {
                        var productValue = metaReader.GetBlobBytes(attribute.Value);
                        rawData.Metadata.Product = System.Text.Encoding.UTF8.GetString(productValue, 3, productValue.Length - 3).TrimEnd('\0');
                    }

                }

                ReflectionDisassembler rd = new(output, CancellationToken.None);
                rd.DetectControlStructure = false;
                rd.WriteAssemblyReferences(metaReader);

                output.WriteLine();
                rd.WriteModuleHeader(peFile);
                output.WriteLine();
                rd.WriteModuleContents(peFile);

                string code = writer.ToString();
                MatchCollection matches = Regex.Matches(code, _searchPattern);

                rawData.Strings = GetStrings(matches);
            }

            return rawData;
        }

        private HashSet<string> GetStrings(MatchCollection matches)
        {
            ConcurrentDictionary<string, byte> data = new();

            Parallel.For(0, matches.Count, i =>
            {
                string val = matches[i].Value.Trim().Trim('"');
                if(val.Length > 1)
                {
                    data.TryAdd(val, 0);
                }
            });

            return new HashSet<string>(data.Keys);
        }
    }
}
