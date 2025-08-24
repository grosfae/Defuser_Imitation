using System.Windows.Media;
using System.Windows;

namespace Defuser_Imitation.Components
{
    //Не используется
    public class SinDrawGeometry
    {
        private double x0;
        private double x1;
        private double scale;
        private uint stepsCount;
        private double multiplier;

        public PathGeometry Sinusoid { get; private set; }

        public double X0 { get => x0; set { x0 = value; Refresh(); } }
        public double X1 { get => x1; set { x1 = value; Refresh(); } }
        public double Scale { get => scale; set { scale = value; Refresh(); } }
        public uint StepsCount { get => stepsCount; set { stepsCount = value; Refresh(); } }
        public double Multiplier { get => multiplier; set { multiplier = value; Refresh(); } }
        private void Refresh()
        {
            uint stepsCount = StepsCount;
            if (stepsCount == 0)
            {
                stepsCount = 1;
            }
            double scale = Scale;
            if (scale <= 0)
            {
                scale = 1;
            }
            double multiplier = Multiplier;
            if (multiplier <= 0)
            {
                multiplier = 1;
            }
            (double x0, double x1) = (X0, X1);
            if (x0 > x1)
            {
                (x0, x1) = (x1, x0);
            }
            double step = (x1 - x0) / stepsCount;

            PathGeometry geometry = new PathGeometry();
            PathFigure figure = new PathFigure();
            geometry.Figures.Add(figure);

            double offset = 0, x = x0;
            figure.StartPoint = new Point(offset, -Math.Sin(x) * scale);

            for (offset += step, x += step; x < x1; offset += step, x += step)
            {
                figure.Segments.Add(new LineSegment(new Point(offset * scale, -Math.Sin(x * multiplier) * scale), true));
            }
            figure.Segments.Add(new LineSegment(new Point((x1 - x0) * scale, -Math.Sin(x1) * scale), true));

            geometry.Freeze();
            Sinusoid = geometry;
        }
    }
}
