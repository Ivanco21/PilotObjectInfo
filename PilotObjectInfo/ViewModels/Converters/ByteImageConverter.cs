using SharpVectors.Runtime;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace PilotObjectInfo.ViewModels.Converters
{
    public class ByteImageConverter : IValueConverter
    {
        public static DrawingImage Convert(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return (DrawingImage)null;
            DrawingGroup drawing;
            if (!SvgConverterHelper.TryConvert(bytes, out drawing))
                return (DrawingImage)null;
            Rect drawingRect = ByteImageConverter.GetDrawingRect(drawing);
            DrawingBrush drawingBrush1 = new ((Drawing)drawing);
            drawingBrush1.Stretch = Stretch.None;
            drawingBrush1.Viewbox = drawingRect;
            drawingBrush1.ViewboxUnits = BrushMappingMode.Absolute;
            DrawingBrush drawingBrush2 = drawingBrush1;
            DrawingImage drawingImage = new ((Drawing)new GeometryDrawing()
            {
                Brush = (Brush)drawingBrush2,
                Geometry = (Geometry)new RectangleGeometry(drawingRect)
            });
            drawingImage.Freeze();
            return drawingImage;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is byte[] bytes ? (object)ByteImageConverter.Convert(bytes) : (object)null;
        }

        public object ConvertBack(
          object value,
          Type targetType,
          object parameter,
          CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static Rect GetDrawingRect(DrawingGroup rootGroup)
        {
            return (ByteImageConverter.GetDrawingRectRecursive(rootGroup) ?? throw new InvalidOperationException("Ошибка при попытке получить размеры drawing group"));
        }

        private static Rect? GetDrawingRectRecursive(DrawingGroup group)
        {
            if (SvgLink.GetKey((DependencyObject)group) == "_SvgDrawingLayer" && group.ClipGeometry is RectangleGeometry clipGeometry)
                return new Rect?(clipGeometry.Bounds);
            foreach (DrawingGroup group1 in group.Children.OfType<DrawingGroup>())
            {
                Rect? drawingRectRecursive = ByteImageConverter.GetDrawingRectRecursive(group1);
                if (drawingRectRecursive.HasValue)
                    return drawingRectRecursive;
            }
            return new Rect?();
        }
    }
}
