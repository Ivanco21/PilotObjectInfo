
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
        /// Позволяет работать с конфигурацией атрибутов
        /// </summary>
        internal static IAttributeFormatParser AttributeFormatParser { get; set; }

        /// <summary>
        /// Обеспечивает работу с вкладками главного окна
        /// </summary>
        internal static ITabServiceProvider TabServiceProvider { get; set; }

    }
}
