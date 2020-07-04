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
        public List<List<System.Drawing.Point>> points = new List<List<System.Drawing.Point>>();
        public List<List<bool>> veryfied = new List<List<bool>>();
        public Equipment.Compatibility MyOwnFirst, MyOwnSecond;
        public bool first = true, second = true;
        public dynamic firstEquip = null, secondEquip = null;


        public void draw(Graphics g, int i, bool reb = true)
        {
            if (points.Count < 2) return;
            if (reb) rebuild();

            points[i][0] = new System.Drawing.Point(firstEquip.locations[i].X + firstEquip.scales[i].X / 2, firstEquip.locations[i].Y + firstEquip.scales[i].Y / 2);
            points[i][points[i].Count - 1] = new System.Drawing.Point(secondEquip.locations[i].X + secondEquip.scales[i].X / 2, secondEquip.locations[i].Y + secondEquip.scales[i].Y / 2);

            System.Drawing.Point prev = points[i][0];
            for (int j = 1; j < points[i].Count - 1; j++)
            {
                if (veryfied[i][j])
                {
                    g.DrawLine(Pens.Black, prev, points[i][j]);
                    prev = points[i][j];
                }
            }
            g.DrawLine(Pens.Black, prev, points[i][points[i].Count - 1]);


            //var t = (drawer)firstEquip;
            //var pa = new Point(t.locations[i].X + t.scales[i].X / 2, t.locations[i].Y + t.scales[i].Y / 2);
            //var q = (drawer)secondEquip;
            //Point dva = new Point(q.locations[i].X + q.scales[i].X / 2, q.locations[i].Y + q.scales[i].Y / 2);
            //g.DrawLine(Pens.Black, pa,dva);

        }
        public void createPoints()
        {
            points = new List<List<System.Drawing.Point>>();
            points.Add(new List<System.Drawing.Point>());

            veryfied = new List<List<bool>>();
            veryfied.Add(new List<bool>());
            var t = (drawer)firstEquip;
            for (int i = 0; i < 4; i++)
            {
                points[i].Add(new System.Drawing.Point(t.locations[i].X + t.scales[i].X / 2, t.locations[i].Y + t.scales[i].Y / 2));
                veryfied[i].Add(true);
                if (i != 3)
                {
                    points.Add(new List<System.Drawing.Point>());
                    veryfied.Add(new List<bool>());
                }
            }
            for (int i = 0; i < 4; i++)
            {
                var q = (drawer)secondEquip;
                System.Drawing.Point dva = new System.Drawing.Point(q.locations[i].X + q.scales[i].X / 2, q.locations[i].Y + q.scales[i].Y / 2);
                double dist = Schemes_Editor.distance(dva, points[i][0]);
                double shag = dist / 10.0;
                System.Drawing.Point prev = points[i][0];
                double angle = Math.Atan((dva.X - (points[i][0].X * 1.0)) / (dva.Y - (points[i][0].Y * 1.0)));
                for (int j = 0; j < 10; j++)
                {
                    points[i].Add(new System.Drawing.Point((int)(prev.X + shag * Math.Sin(angle)), (int)(prev.Y + shag * Math.Cos(angle))));
                    veryfied[i].Add(false);
                    prev = new System.Drawing.Point((int)(prev.X + shag * Math.Sin(angle)), (int)(prev.Y + shag * Math.Cos(angle)));
                }
                points[i].Add(dva);
                veryfied[i].Add(true);
            }

        }

        internal void rebuild()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < points[i].Count; j++)
                {
                    if (!veryfied[i][j])
                    {
                        veryfied[i].RemoveAt(j);
                        points[i].RemoveAt(j);
                        j--;
                    }
                }
            }
            List<List<System.Drawing.Point>> newlist = new List<List<System.Drawing.Point>>();
            for (int i = 0; i < 4; i++)
            {
                newlist.Add(new List<System.Drawing.Point>());
                veryfied[i] = new List<bool>();
                veryfied[i].Add(true);
                newlist[i].Add(points[i][0]);

                for (int j = 1; j < points[i].Count; j++)
                {
                    double dist = Schemes_Editor.distance(points[i][j], points[i][j - 1]);
                    double shag = dist / 10.0;
                    System.Drawing.Point prev = points[i][j - 1];
                    double angle = Math.Atan((points[i][j].X - (points[i][j - 1].X * 1.0)) / (points[i][j].Y - (points[i][j - 1].Y * 1.0)));
                    for (int k = 0; k < 10; k++)
                    {
                        newlist[i].Add(new System.Drawing.Point((int)(prev.X + shag * Math.Sin(angle)), (int)(prev.Y + shag * Math.Cos(angle))));
                        veryfied[i].Add(false);
                        prev = new System.Drawing.Point((int)(prev.X + shag * Math.Sin(angle)), (int)(prev.Y + shag * Math.Cos(angle)));
                    }
                    veryfied[i].Add(true);
                    newlist[i].Add(points[i][j]);
                }

            }
            points = newlist;
        }
    }
}
