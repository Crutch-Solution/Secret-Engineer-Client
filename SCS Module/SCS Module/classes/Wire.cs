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
        public string[] labels = null;

        public Vinoska[] vinoska = new Vinoska[] { null,null,null,null};

        public int globalId, localID;

        public List<List<Point>> points = new List<List<Point>>();

        public List<List<Point>> vertexes = new List<List<Point>>();

        public Equipment.Compatibility MyOwnFirst, MyOwnSecond;

        public Equipment.Compatibility OtherFirst, OtherSecond;

        public bool isFirstSeized = false, isSecondSeized = false;

        public drawer firstEquip = null, secondEquip = null;


        public void createVinosku(Point p, int index)
        {
            //поиск целевого шкафа
            vinoska[index] = new Vinoska(labels[index], p, new Point(p.X, p.Y-100), new Point(p.X+100, p.Y - 100));
        }
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

            if (vinoska != null && vinoska[index] !=null)
            {
                g.DrawLines(Pens.Blue, new Point[] { vinoska[index].startPoint, vinoska[index].vertex1, vinoska[index].vertex2 });

                if (vinoska[index].vertex1.X < vinoska[index].vertex2.X)
                    g.DrawString(labels[index], new Font("Arial", 14), Brushes.DarkGreen, vinoska[index].vertex1.X, vinoska[index].vertex1.Y - 30);
                else
                    g.DrawString(labels[index], new Font("Arial", 14), Brushes.DarkGreen, vinoska[index].vertex2.X, vinoska[index].vertex1.Y - 30);
            }
            //foreach (var i in vertexes[index])
            //    g.DrawEllipse(Pens.Blue, i.X - 1, i.Y - 1, 2, 2);

        }
        public void createPoints()
        {
            if (labels == null)
            {
                string a = Schemes_Editor.mainList.Find(x => x.id == globalId).name;
                labels = new string[] { a, a , a , a };
            }
            points = new List<List<Point>>();
            vertexes = new List<List<Point>>();
            for (int i = 0; i < 4; i++)
            {
                if (i == 2)
                {
                    points.Add(null);
                    vertexes.Add(null);
                }
                else
                {
                    points.Add(new List<Point>());
                    vertexes.Add(new List<Point>());
                    points[i].Add(new Point(firstEquip.locations[i].X + firstEquip.scales[i].X / 2, firstEquip.locations[i].Y + firstEquip.scales[i].Y / 2));
                    points[i].Add(new Point(secondEquip.locations[i].X + secondEquip.scales[i].X / 2, secondEquip.locations[i].Y + secondEquip.scales[i].Y / 2));
                }
            }
        }
        public int insertPoint(Point a, int index)
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
                        if(getted.X == a.X && getted.Y == a.Y)
                        {
                            points[index].Insert(i + 1, a);
                            return i + 1;
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < localdistance / step; j++)
                    {
                       Point getted = new Point((int)(points[index][i].X + j * step * Math.Cos(angle)), (int)(points[index][i].Y + j * step * Math.Sin(angle)));
                        if (getted.X == a.X && getted.Y == a.Y)
                        {
                            points[index].Insert(i + 1, a);
                            return i + 1;
                        }
                    }
                }
            }
            return -1;
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
         for(int i=1;i< points[index].Count-1;i++)
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
