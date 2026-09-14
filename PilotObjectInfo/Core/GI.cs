using System.Collections.Generic;
using System.Linq;
using Ascon.Pilot.SDK;

namespace PilotObjectInfo.Core
{
    internal class GI
    {
        /// <summary>
        /// Редактор объектов Pilot
        /// </summary>
        internal static IObjectModifier Modifier { get; set; }

        /// <summary>
        /// Репозиторий Pilot
        /// </summary>
        internal static IObjectsRepository Repository { get; set; }

        /// <summary>
        /// Интерфейс поиска
        /// </summary>
        internal static ISearchService SearchService { get; set; }

        /// <summary>
        /// Обеспечивает работу с телами файлов
        /// </summary>
        internal static IFileProvider FileProvider { get; set; }
        
        /// <summary>
        /// Обеспечивает работу с вкладками главного окна
        /// </summary>
        internal static ITabServiceProvider TabServiceProvider { get; set; }
        internal static List<IAttribute> GetAllAttrs()
        {
            var types = Repository.GetTypes();
            if (types == null)
                return new List<IAttribute>();

            return types
                .Where(t => t?.Attributes != null)
                .SelectMany(t => t.Attributes)
                .Where(a => a != null)
                .ToList();
        }
    }
}
