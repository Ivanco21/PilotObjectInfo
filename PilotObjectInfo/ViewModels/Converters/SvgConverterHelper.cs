using System;
using System.IO;
using System.Windows.Media;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace PilotObjectInfo.ViewModels.Converters
{
    public class SvgConverterHelper
    {
        public static bool TryConvert(byte[] bytes, out DrawingGroup drawing)
        {
            drawing = (DrawingGroup)null;
            if (bytes == null || bytes.Length == 0)
                return false;
            using (MemoryStream svgStream = new (bytes))
            {
                try
                {
                    FileSvgReader fileSvgReader = new (new WpfDrawingSettings()
                    {
                        IncludeRuntime = true,
                        TextAsGeometry = false,
                        EnsureViewboxPosition = false
                    });
                    drawing = fileSvgReader.Read((Stream)svgStream);
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }
    }
}
