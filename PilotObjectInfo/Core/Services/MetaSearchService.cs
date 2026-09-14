using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ascon.Pilot.SDK;
using PilotObjectInfo.Dto;

namespace PilotObjectInfo.Core.Services
{
    /// <summary>
    /// Сервис конвертации исходного текста в meta-code.
    /// </summary>
    internal class MetaSearchService
    {
        private static readonly char[] DIGITS = "0123456789".ToCharArray();
        private static readonly char UNDERSCORE = '_';

        #region Naming Exceptions
        /// <summary>
        /// для более лаконичного отображения, для части Pilot entity
        /// </summary>
        private static readonly Dictionary<string, string> NamingExceptions = new(StringComparer.OrdinalIgnoreCase)
        {
            ["projectfolder"] = "PROJECT_FOLDER",
            ["projectfolder_collection"] = "PROJECT_FOLDER_COLLECTION",
            ["otd_doc_ii"] = "DOC_II",
            ["otd_doc_ii_archive"] = "DOC_II_ARCHIVE",
            ["7_approval_section_doc_pd"] = "DOC_PD",
            ["7_approval_section_doc_pd_archive"] = "DOC_PD_ARCHIVE",
            ["7_approval_section_doc_pp"] = "DOC_PP",
            ["7_approval_section_doc_pp_archive"] = "DOC_PP_ARCHIVE",
            ["8_approval_section_doc_rd"] = "DOC_RD",
            ["8_approval_section_doc_rd_archive"] = "DOC_RD_ARCHIVE",
            ["8_approval_section_doc_rd_multi"] = "DOC_RD_MULTI",
            ["8_approval_section_doc_rd_multi_archive"] = "DOC_RD_MULTI_ARCHIVE"
        };
        #endregion


        private readonly LatinWordExtractorService _latinWordExtractorService;

        internal MetaSearchService(LatinWordExtractorService latinWordExtractorService = null)
        {
            _latinWordExtractorService = latinWordExtractorService ?? new LatinWordExtractorService();
        }
        
        internal string Convert(string sourceText)
        {
            if (string.IsNullOrWhiteSpace(sourceText))
                return string.Empty;

            string[] words = _latinWordExtractorService.ExtractWords(sourceText);
            MinMetaGeneratorService metaGeneratorService = new();
            List<MinMetaDto> allMeta = new();

            allMeta.AddRange(metaGeneratorService.CreateMany(GI.GetAllAttrs()));
            allMeta.AddRange(metaGeneratorService.CreateMany(GI.Repository.GetTypes()));
            allMeta.AddRange(metaGeneratorService.CreateMany(GI.Repository.GetUserStates()));

            return Convert(words, allMeta);
        }

        string Convert(string[] words, IEnumerable<MinMetaDto> allMeta)
        {
            if (words == null || words.Length == 0 || allMeta == null)
                return string.Empty;
            
            var metaDict = new Dictionary<string, List<MinMetaDto>>(StringComparer.OrdinalIgnoreCase);

            foreach (var meta in allMeta)
            {
                if (meta == null || string.IsNullOrWhiteSpace(meta.Name))
                    continue;

                if (!metaDict.TryGetValue(meta.Name, out var list))
                {
                    list = new List<MinMetaDto>();
                    metaDict[meta.Name] = list;
                }

                var existing = list.FirstOrDefault(x => x.MetaClass == meta.MetaClass &&
                                                        string.Equals(x.Name, meta.Name, StringComparison.OrdinalIgnoreCase) &&
                                                        string.Equals(x.Title ?? string.Empty, meta.Title ?? string.Empty, StringComparison.OrdinalIgnoreCase));
                if (existing == null)
                {
                    list.Add(meta);
                }
            }

            List<MinMetaDto> matchedTypes = new();
            List<MinMetaDto> matchedAttrs = new();
            List<MinMetaDto> matchedStates = new();

            foreach (string word in words)
            {
                if (metaDict.TryGetValue(word, out var matches))
                {
                    foreach (var match in matches)
                    {
                        switch (match.MetaClass)
                        {
                            case MetaClass.PType:
                                matchedTypes.Add(match);
                                break;

                            case MetaClass.PAttribute:
                                matchedAttrs.Add(match);
                                break;

                            case MetaClass.PState:
                                matchedStates.Add(match);
                                break;
                        }
                    }
                }
            }

            List<string> classBlocks = new();

            if (matchedTypes.Count > 0)
                classBlocks.Add(BuildClass("TypeConsts", matchedTypes));

            if (matchedAttrs.Count > 0)
                classBlocks.Add(BuildClass("AttrConsts", matchedAttrs));

            if (matchedStates.Count > 0)
                classBlocks.Add(BuildClass("StateConsts", matchedStates));

            return string.Join(Environment.NewLine + Environment.NewLine, classBlocks);
        }

        private static string BuildClass(string className, List<MinMetaDto> items)
        {
            if (items == null || items.Count == 0)
                return string.Empty;

            var sb = new StringBuilder();
            sb.AppendLine($"internal class {className}");
            sb.AppendLine("{");
            sb.AppendLine($"    private {className}() {{ }}");
            sb.AppendLine();

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                string constName = DoFormatValue(item.Name);
                string summary = string.IsNullOrWhiteSpace(item.Title) ? item.Name : item.Title;

                sb.AppendLine("    /// <summary>");
                var lines = summary.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                foreach (var line in lines)
                {
                    sb.AppendLine($"    /// {line}");
                }
                sb.AppendLine("    /// </summary>");
                sb.AppendLine($"    internal const string {constName} = \"{item.Name}\";");
                sb.AppendLine();
            }

            sb.Append("}");
            return sb.ToString();
        }

        private static string DoFormatValue(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            string trimmed = input.Trim();

            if (NamingExceptions.TryGetValue(trimmed, out string exceptionName))
                return exceptionName;

            string working = trimmed.TrimStart(DIGITS).TrimStart(UNDERSCORE);
            if (working.Length == 0)
                working = "_" + trimmed;

            return working.ToUpper();
        }
    }
}
