using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace PilotObjectInfo.ViewModels.Converters
{
    /// <summary>
    /// Like GraphShape's EdgeRouteToPathConverter, but draws arrows on both ends
    /// </summary>
    public sealed class BidirectionalEdgeRouteToPathConverter : IMultiValueConverter
    {
        private const double ArrowLength = 8;
        private const double ArrowWidthFactor = 0.32;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 9)
                return Geometry.Empty;

            var sourcePos = new Point(GetDouble(values[0]), GetDouble(values[1]));
            var sourceSize = new Size(GetDouble(values[2]), GetDouble(values[3]));
            var targetPos = new Point(GetDouble(values[4]), GetDouble(values[5]));
            var targetSize = new Size(GetDouble(values[6]), GetDouble(values[7]));
            var route = values[8] != DependencyProperty.UnsetValue
                ? values[8] as Point[]
                : null;
            bool isBidirectional = values.Length > 9 && values[9] is true;

            bool hasRoute = route != null && route.Length > 0;

            Point p1 = GetClippingPoint(sourceSize, sourcePos, hasRoute ? route[0] : targetPos);
            Point p2 = GetClippingPoint(targetSize, targetPos, hasRoute ? route[route.Length - 1] : sourcePos);

            var segments = new PathSegment[1 + (hasRoute ? route.Length : 0)];
            if (hasRoute)
            {
                for (int i = 0; i < route.Length; i++)
                    segments[i] = new LineSegment(route[i], true);
            }

            Point pLast = hasRoute ? route[route.Length - 1] : p1;
            Vector vTarget = pLast - p2;
            if (vTarget.Length > 0)
                vTarget = vTarget / vTarget.Length * ArrowLength;
            var nTarget = new Vector(-vTarget.Y, vTarget.X) * ArrowWidthFactor;

            Point pFirst = hasRoute ? route[0] : p2;
            Vector vSource = pFirst - p1;
            if (vSource.Length > 0)
                vSource = vSource / vSource.Length * ArrowLength;
            var nSource = new Vector(-vSource.Y, vSource.X) * ArrowWidthFactor;

            Point lineStart = isBidirectional ? p1 + vSource : p1;
            segments[segments.Length - 1] = new LineSegment(p2 + vTarget, true);

            int figureCount = isBidirectional ? 3 : 2;
            var pathCollection = new PathFigureCollection(figureCount)
            {
                new PathFigure(lineStart, segments, false),
                new PathFigure(
                    p2,
                    new PathSegment[]
                    {
                        new LineSegment(p2 + vTarget - nTarget, true),
                        new LineSegment(p2 + vTarget + nTarget, true)
                    },
                    true)
            };

            if (isBidirectional)
            {
                pathCollection.Add(new PathFigure(
                    p1,
                    new PathSegment[]
                    {
                        new LineSegment(p1 + vSource - nSource, true),
                        new LineSegment(p1 + vSource + nSource, true)
                    },
                    true));
            }

            return new PathGeometry(pathCollection);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();

        private static double GetDouble(object value) =>
            value != null && value != DependencyProperty.UnsetValue ? (double)value : 0.0;

        /// <summary>
        /// Port of GraphShape.Algorithms.Layout.LayoutUtils.GetClippingPoint.
        /// </summary>
        private static Point GetClippingPoint(Size size, Point center, Point p)
        {
            var sides = new[]
            {
                (center.X - size.Width / 2.0 - p.X) / (center.X - p.X),
                (center.Y - size.Height / 2.0 - p.Y) / (center.Y - p.Y),
                (center.X + size.Width / 2.0 - p.X) / (center.X - p.X),
                (center.Y + size.Height / 2.0 - p.Y) / (center.Y - p.Y)
            };

            double fi = 0;
            for (int i = 0; i < 4; ++i)
            {
                if (sides[i] <= 1)
                    fi = Math.Max(fi, sides[i]);
            }

            if (IsZero(fi))
            {
                fi = double.PositiveInfinity;
                for (int i = 0; i < 4; ++i)
                    fi = Math.Min(fi, Math.Abs(sides[i]));
                fi *= -1;
            }

            return p + fi * (center - p);
        }

        private static bool IsZero(double value) => Math.Abs(value) < 0.00001;
    }
}
