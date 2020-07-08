using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCS_Module
{
    public class Wire
    {
        public List<List<Point>> points = new List<List<Point>>();

        public List<List<Point>> vertexes = new List<List<Point>>();

        public Equipment.Compatibility MyOwnFirst, MyOwnSecond;

        public Equipment.Compatibility OtherFirst, OtherSecond;

        public bool isFirstSeized = false, isSecondSeized = false;

        public drawer firstEquip = null, secondEquip = null;



        public void draw(Graphics g, int index)
        {
            if (points.Count < 2) return;
            rebuild(index);

            for (int i = 0; i < points[index].Count - 1; i++)
                g.DrawLine(new Pen(Brushes.Red, 2), points[index][i], points[index][i + 1]);

        }
        public void createPoints()
        {
            points = new List<List<Point>>();
            vertexes = new List<List<Point>>();
            for (int i = 0; i < 4; i++)
            {
                if (i == 2) points.Add(null);
                else
                {
                    points.Add(new List<Point>());
                    vertexes.Add(new List<Point>());
                    points[i].Add(new Point(firstEquip.locations[i].X + firstEquip.scales[i].X / 2, firstEquip.locations[i].Y + firstEquip.scales[i].Y / 2));
                }
            }
        }
        public void rebuild(int index)
        {
            points[index][0] = new Point(firstEquip.locations[index].X + firstEquip.scales[index].X / 2, firstEquip.locations[index].Y + firstEquip.scales[index].Y / 2);
            points[index][points[index].Count - 1] = new Point(secondEquip.locations[index].X + secondEquip.scales[index].X / 2, secondEquip.locations[index].Y + secondEquip.scales[index].Y / 2);

            //calculate length
            double step = 0;
            for (int i = 0; i < points[index].Count - 1; i++)
                step += Schemes_Editor.distance(points[index][i], points[index][i + 1]);
            step /= 100.0;

            vertexes[index].Clear();

            for (int i = 0; i < points[index].Count - 1; i++)
            {
                double localdistance = Schemes_Editor.distance(points[index][i], points[index][i + 1]);
                double angle = Math.Atan((points[index][i + 1].Y - points[index][i].Y) / (points[index][i + 1].X - points[index][i].X));

                for (int j = 0; j < localdistance / step; j++)
                {
                    vertexes[index].Add(new Point((int)(points[index][i].X + step * Math.Cos(angle)), (int)(points[index][i].Y + step * Math.Sin(angle))));
                }
            }
        }
        public Point inside(int index, Point target)
        {
            //find shortest dist
            double distance = double.MaxValue, buff;
            Point result = new Point(-1,-1);
            foreach (var i in vertexes[index])
            {
                buff = Schemes_Editor.distance(i, target);
                if (buff < 30 && buff < distance)
                {
                    result = i;
                    distance = buff;
                }
            }
            return result;
        }
    }
}
