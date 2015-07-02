using System.Windows;
using System.Windows.Media;

namespace CoreUtils
{
    public static class Types
    {
        private const double MaxArrowLengthPercent = 0.3; // factor that determines how the arrow is shortened for very short lines
        private const double LineArrowLengthFactor = 3.73205081; // 15 degrees arrow:  = 1 / Math.Tan(15 * Math.PI / 180); 

        public static PointCollection CreateArrowPointCollection(Point startPoint, Point endPoint, double lineWidth)
        {
            var source = new Point(startPoint.X + 0.5, startPoint.Y + 0.5);
            var destination = new Point(endPoint.X + 0.5, endPoint.Y + 0.5);

            Vector direction = source - destination;

            Vector normalizedDirection = direction;
            normalizedDirection.Normalize();

            Vector normalizedlineWidenVector = new Vector(-normalizedDirection.Y, normalizedDirection.X); // Rotate by 90 degrees
            Vector lineWidenVector = normalizedlineWidenVector * lineWidth * 0.5;

            double lineLength = direction.Length;

            double defaultArrowLength = lineWidth * LineArrowLengthFactor;

            // Prepare usedArrowLength
            // if the length is bigger than 1/3 (_maxArrowLengthPercent) of the line length adjust the arrow length to 1/3 of line length

            double usedArrowLength;
            if (lineLength * MaxArrowLengthPercent < defaultArrowLength)
                usedArrowLength = lineLength * MaxArrowLengthPercent;
            else
                usedArrowLength = defaultArrowLength;

            // Adjust arrow thickness for very thick lines
            double arrowWidthFactor;
            //if (lineWidth <= 1.5)
            //    arrowWidthFactor = 3;
            //else if (lineWidth <= 2.66)
            //    arrowWidthFactor = 4;
            //else
                arrowWidthFactor = 1.5 * lineWidth;

            Vector arrowWidthVector = normalizedlineWidenVector * arrowWidthFactor;


            // Now we have all the vectors so we can create the arrow shape positions
            var pointCollection = new PointCollection(7);

            Point endArrowCenterPosition = destination - (normalizedDirection * usedArrowLength);

            pointCollection.Add(destination); // Start with tip of the arrow
            pointCollection.Add(endArrowCenterPosition + arrowWidthVector);
            pointCollection.Add(endArrowCenterPosition + lineWidenVector);
            pointCollection.Add(source + lineWidenVector);
            pointCollection.Add(source - lineWidenVector);
            pointCollection.Add(endArrowCenterPosition - lineWidenVector);
            pointCollection.Add(endArrowCenterPosition - arrowWidthVector);

            return pointCollection;
        }
    }
}
