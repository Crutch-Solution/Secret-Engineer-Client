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



        public void draw(Graphics g, int index, bool selected = false)
        {
            if (points.Count < 2) return;
            rebuild(index);
            if (selected)
                for (int i = 0; i < points[index].Count - 1; i++)
                    g.DrawLine(new Pen(Brushes.Green, 2), points[index][i], points[index][i + 1]);
            else
                for (int i = 0; i < points[index].Count - 1; i++)
                    g.DrawLine(new Pen(Brushes.Red, 1), points[index][i], points[index][i + 1]);

            //foreach (var i in vertexes[index])
            //    g.DrawEllipse(Pens.Blue, i.X - 1, i.Y - 1, 2, 2);

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
                    points[i].Add(new Point(secondEquip.locations[i].X + secondEquip.scales[i].X / 2, secondEquip.locations[i].Y + secondEquip.scales[i].Y / 2));
                }
            }
        }
        public void insertPoint(Point a, int index)
        {
            //найти между каким точками раположена
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
                double angle = Math.Atan((points[index][i + 1].Y - points[index][i].Y) / ((points[index][i + 1].X - points[index][i].X) * 1.0));
                if (points[index][i].X > points[index][i + 1].X)
                {
                    for (int j = 0; j < localdistance / step; j++)
                    {
                        Point getted = new Point((int)(points[index][i].X - j * step * Math.Cos(angle)), (int)(points[index][i].Y - j * step * Math.Sin(angle)));

                    }
                }
                else
                {
                    for (int j = 0; j < localdistance / step; j++)
                    {
                       Point getted = new Point((int)(points[index][i].X + j * step * Math.Cos(angle)), (int)(points[index][i].Y + j * step * Math.Sin(angle)));

                    }
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
                double angle = Math.Atan((points[index][i + 1].Y - points[index][i].Y) / ((points[index][i + 1].X - points[index][i].X)*1.0));
                if (points[index][i].X > points[index][i + 1].X)
                {
                    for (int j = 0; j < localdistance / step; j++)
                    {
                        vertexes[index].Add(new Point((int)(points[index][i].X - j * step * Math.Cos(angle)), (int)(points[index][i].Y - j * step * Math.Sin(angle))));
                    }
                }
                else
                {
                    for (int j = 0; j < localdistance / step; j++)
                    {
                        vertexes[index].Add(new Point((int)(points[index][i].X + j * step * Math.Cos(angle)), (int)(points[index][i].Y + j * step * Math.Sin(angle))));
                    }
                }
            }
        }
        public selectedVertex inside(int index, Point target)
        {
            //find shortest dist
            selectedVertex result = new selectedVertex();
            double distance = double.MaxValue, buff;
            result.vertex = new Point(-1, -1);
            foreach (var i in vertexes[index])
            {
                buff = Schemes_Editor.distance(i, target);
                if (buff < 30 && buff < distance)
                {
                    result.vertex = i;
                    distance = buff;
                }
            }
            bool firstTime = true;
         for(int i=0;i< points[index].Count;i++)
            {
                buff = Schemes_Editor.distance(points[index][i], target);
                if (buff < 30 && (buff < distance || firstTime))
                {
                    firstTime = false;
                    distance = buff;
                    result.vertex = points[index][i];
                    result.isExists = true;
                    result.ExistingIndex = i;
                }
            }
            return result;
        }
    }
    public class selectedVertex
    {
        public Point vertex;
        public bool isExists = false;
        public int ExistingIndex =0;
    }
}
