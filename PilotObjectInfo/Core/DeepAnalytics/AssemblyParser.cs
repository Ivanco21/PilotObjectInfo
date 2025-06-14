using Ascon.Pilot.VisualElements.Tools.DragAndDrop;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using PilotObjectInfo.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration.Assemblies;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace PilotObjectInfo.Core.DeepAnalytics
{
    internal partial class AssemblyParser
    {

        private const string EXT_TARGET = ".ext2.dll";
        const string _searchPattern = @"([""])(?:(?=(\\?))\2.)*?\1";
        const string _decompileError = @"Decompilation failed: ";


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

                ReflectionDisassembler rd = new(output, CancellationToken.None);
                rd.DetectControlStructure = false;
                //rd.WriteAssemblyReferences(metaReader);

                output.WriteLine();
                rd.WriteModuleHeader(peFile);
                output.WriteLine();
                rd.WriteModuleContents(peFile);

                string code = writer.ToString();
                MatchCollection matches = Regex.Matches(code, _searchPattern);

                rawData.Strings = GetStrings(matches);
            }

            AssemblyMetaDataDTO assemblyMeta = new();
            assemblyMeta = GetMetaData(path);
            rawData.Metadata.Id = assemblyMeta.Id;
            rawData.Metadata.InternalName = assemblyMeta.InternalName;
            rawData.Metadata.Version = assemblyMeta.Version;
            rawData.Metadata.Product = assemblyMeta.Product;
            rawData.Metadata.Company = assemblyMeta.Company;

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

        internal CodeStateModel GetCodeState(string path, string searchTerm)
        {
            CodeStateModel codeState = new();
            string CStext = DecompileAssemblyToCSharp(path);
            if (CStext.StartsWith(_decompileError))
            {
                return codeState;
            }

            codeState.CodeParts = GetContextAroundMatches(CStext, searchTerm);
            codeState.Path = path;
            AssemblyMetaDataDTO assemblyMeta = new();
            assemblyMeta = GetMetaData(path);

            codeState.Id = assemblyMeta.Id;
            codeState.InternalName = assemblyMeta.InternalName;
            codeState.Version = assemblyMeta.Version;
            codeState.Product = assemblyMeta.Product;
            codeState.Company = assemblyMeta.Company;

            return codeState;
        }


        internal string DecompileAssemblyToCSharp(string assemblyPath)
        {
            var settings = new DecompilerSettings
            {
                ThrowOnAssemblyResolveErrors = false,
                RemoveDeadCode = true,
                UsingDeclarations = true,
                AlwaysUseBraces = true,
                ShowXmlDocumentation = false,
            };

            using var peFile = new PEFile(assemblyPath);
            var resolver = new UniversalAssemblyResolver(assemblyPath, false, peFile.DetectTargetFrameworkId());
            var decompiler = new CSharpDecompiler(peFile, resolver, settings);

            try
            {
                // Decompile the entire assembly to C#
                return decompiler.DecompileWholeModuleAsString();
            }
            catch (Exception ex)
            {
                return $"{_decompileError}{ex.Message}";
            }
        }

        private AssemblyMetaDataDTO GetMetaData(string assemblyPath)
        {
            using var peFile = new PEFile(assemblyPath);
            var metaReader = peFile.Metadata;
            AssemblyDefinition assemblyDefinition = peFile.Metadata.GetAssemblyDefinition();

            Guid id = metaReader.GetGuid(metaReader.GetModuleDefinition().Mvid);
            string internalName = assemblyDefinition.GetAssemblyName().Name;
            string version = assemblyDefinition.GetAssemblyName().Version.ToString();
            string product = string.Empty;
            string company = string.Empty;

            foreach (CustomAttributeHandle attributeHandle in assemblyDefinition.GetCustomAttributes())
            {
                var attribute = metaReader.GetCustomAttribute(attributeHandle);
                var attrType = attribute.GetAttributeType(metaReader);

                if (attrType.GetFullTypeName(metaReader).Name == "AssemblyProductAttribute")
                {
                    var productValue = metaReader.GetBlobBytes(attribute.Value);
                    product = System.Text.Encoding.UTF8.GetString(productValue, 3, productValue.Length - 3).TrimEnd('\0');
                }
                if (attrType.GetFullTypeName(metaReader).Name == "AssemblyCompanyAttribute")
                {
                    var productValue = metaReader.GetBlobBytes(attribute.Value);
                    company = System.Text.Encoding.UTF8.GetString(productValue, 3, productValue.Length - 3).TrimEnd('\0');
                }

            }

            AssemblyMetaDataDTO assemblyMeta = new();
            assemblyMeta.Id = id;
            assemblyMeta.InternalName = internalName;
            assemblyMeta.Version = version;
            assemblyMeta.Product = product;
            assemblyMeta.Company = company;

            return assemblyMeta;
        }

        private List<string> GetContextAroundMatches(string textCS, string searchTerm,
                                                            int linesBefore = 2, int linesAfter = 7, 
                                                            StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            string[] allLines = textCS.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            var matchingIndices = allLines
                .Select((line, index) => new { line, index })
                .Where(x => x.line.IndexOf(searchTerm, comparison) >= 0)
                .Select(x => x.index)
                .ToList();

            List<string> result = new();
            foreach (int matchIndex in matchingIndices)
            {
                int start = Math.Max(0, matchIndex - linesBefore);
                int end = Math.Min(allLines.Length - 1, matchIndex + linesAfter);

                string oneSearchRes =string.Empty;
                for (int i = start; i <= end; i++)
                {
                    oneSearchRes += allLines[i] + Environment.NewLine;
                }
                result.Add(oneSearchRes);
            }

            return result;
        }
    }
}
