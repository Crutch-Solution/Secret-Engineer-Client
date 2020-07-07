using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SCS_Module
{
    public class inboxes : drawer
    {
        public string boxLabel = "не задано", conLabel = "не задано", strLabel = "не задано";
        public List<int> seized = new List<int>();
        public bool isShowingOnConnectionScheme = true;
        public List<UsedInterface> listINterfaces = new List<UsedInterface>();
        public bool inbox = false;
        public int numberOfUnits = -1;
        public override void move(System.Drawing.Point offset, int scheme)
        {
            if (vinoska != null)
            {
                vinoska.startPoint = new System.Drawing.Point(locations[scheme].X + scales[scheme].X / 2, locations[scheme].Y + scales[scheme].Y / 2);

                vinoska.vertex1 = new System.Drawing.Point(offset.X + locations[scheme].X - vinoska.vertex1.X, offset.Y + locations[scheme].Y - vinoska.vertex1.Y);
                vinoska.vertex2 = new System.Drawing.Point(offset.X + locations[scheme].X - vinoska.vertex2.X, offset.Y + locations[scheme].Y - vinoska.vertex2.Y);
            }
            locations[scheme] = offset;

        }
        public override bool inside(System.Drawing.Point a, int scheme)
        {
            bool result = false;
            if (a.X > locations[scheme].X && a.X < locations[scheme].X + scales[scheme].X)
                if (a.Y > locations[scheme].Y && a.Y < locations[scheme].Y + scales[scheme].Y)
                    result = true;
            return result;
        }
        public override void drawBox(Graphics g)
        {
            int localSheetIndex = 2;
            List<Equipment.VectorPic> list = new List<Equipment.VectorPic>();
            if (Schemes_Editor.mainList.Find(x => x.id == globalId).inPlacementScheme != null)
                list.Add(Schemes_Editor.mainList.Find(x => x.id == globalId).inPlacementScheme.copy());
            else
                list.Add(null);

            if (Schemes_Editor.mainList.Find(x => x.id == globalId).inConnectionScheme != null)
                list.Add(Schemes_Editor.mainList.Find(x => x.id == globalId).inConnectionScheme.copy());
            else
                list.Add(null);

            if (Schemes_Editor.mainList.Find(x => x.id == globalId).inBox != null)
                list.Add(Schemes_Editor.mainList.Find(x => x.id == globalId).inBox.copy());
            else
                list.Add(null);

            if (Schemes_Editor.mainList.Find(x => x.id == globalId).inStructural != null)
                list.Add(Schemes_Editor.mainList.Find(x => x.id == globalId).inStructural.copy());
            else
                list.Add(null);

            if (list[localSheetIndex] != null)
            {
                Equipment.Point WidthHeight = list[localSheetIndex].GetProp();
                float Xprop = WidthHeight.X / (scales[localSheetIndex].X * 1.0f),
                    Yprop = WidthHeight.Y / (scales[localSheetIndex].Y * 1.0f);
                list[localSheetIndex].divide(Xprop, Yprop);

                foreach (var j in list[localSheetIndex].circles)
                    g.DrawEllipse(Pens.Black, locations[localSheetIndex].X + (float)j.center.X - (float)j.radiusX, locations[localSheetIndex].Y + (float)j.center.Y - (float)j.radiusY, (float)j.radiusX * 2, (float)j.radiusY * 2);

                foreach (var j in list[localSheetIndex].polyLines)
                {
                    for (int k = 0; k < j.Count - 1; k++)
                    {
                        g.DrawLine(Pens.Black, j[k].X + locations[localSheetIndex].X, j[k].Y + locations[localSheetIndex].Y, j[k + 1].X + locations[localSheetIndex].X, j[k + 1].Y + locations[localSheetIndex].Y);
                    }
                }
            }
            if (vinoska != null)
            {
                g.DrawLines(Pens.Blue, new System.Drawing.Point[] { vinoska.startPoint, vinoska.vertex1, vinoska.vertex2 });
                g.DrawString(vinoska.text, new Font("Arial", 14), Brushes.DarkGreen, vinoska.vertex1.X, vinoska.vertex1.Y - 30);
            }
        }

        public override void createVinosku()
        {
            if (inbox)
            {
                //поиск целевого шкафа
                int index = ((boxes)Schemes_Editor.mainWorkList.Find(x => x is boxes && ((boxes)x).equipInside.Exists(y => y.localID == localID))).equipInside.IndexOf(this) + 1;
                vinoska = new Vinoska(index.ToString(), new System.Drawing.Point(locations[2].X + scales[2].X / 2, locations[2].Y + scales[2].Y / 2), locations[Schemes_Editor.sheetIndex], new System.Drawing.Point(locations[Schemes_Editor.sheetIndex].X + 30, locations[Schemes_Editor.sheetIndex].Y));
            }
        }

        public override void drawPlace(Graphics g)
        {

        }

        public override void drawCon(Graphics g)
        {
            if (seized.Count == 0)
                foreach (var i in Schemes_Editor.mainList.Find(x => x.id == globalId).compatibilities)
                    seized.Add(0);

            int localSheetIndex = 1;
            List<Equipment.VectorPic> list = new List<Equipment.VectorPic>();
            if (Schemes_Editor.mainList.Find(x => x.id == globalId).inPlacementScheme != null)
                list.Add(Schemes_Editor.mainList.Find(x => x.id == globalId).inPlacementScheme.copy());
            else
                list.Add(null);

            if (Schemes_Editor.mainList.Find(x => x.id == globalId).inConnectionScheme != null)
                list.Add(Schemes_Editor.mainList.Find(x => x.id == globalId).inConnectionScheme.copy());
            else
                list.Add(null);

            if (Schemes_Editor.mainList.Find(x => x.id == globalId).inBox != null)
                list.Add(Schemes_Editor.mainList.Find(x => x.id == globalId).inBox.copy());
            else
                list.Add(null);

            if (Schemes_Editor.mainList.Find(x => x.id == globalId).inStructural != null)
                list.Add(Schemes_Editor.mainList.Find(x => x.id == globalId).inStructural.copy());
            else
                list.Add(null);

            if (list[localSheetIndex] != null)
            {
                Equipment.Point WidthHeight = list[localSheetIndex].GetProp();
                float Xprop = WidthHeight.X / (scales[localSheetIndex].X * 1.0f),
                    Yprop = WidthHeight.Y / (scales[localSheetIndex].Y * 1.0f);
                list[localSheetIndex].divide(Xprop, Yprop);

                foreach (var j in list[localSheetIndex].circles)
                    g.DrawEllipse(Pens.Black, locations[localSheetIndex].X + (float)j.center.X - (float)j.radiusX, locations[localSheetIndex].Y + (float)j.center.Y - (float)j.radiusY, (float)j.radiusX * 2, (float)j.radiusY * 2);

                foreach (var j in list[localSheetIndex].polyLines)
                {
                    for (int k = 0; k < j.Count - 1; k++)
                    {
                        g.DrawLine(Pens.Black, j[k].X + locations[localSheetIndex].X, j[k].Y + locations[localSheetIndex].Y, j[k + 1].X + locations[localSheetIndex].X, j[k + 1].Y + locations[localSheetIndex].Y);
                    }
                }

                StringFormat f = new StringFormat();
                f.Alignment = StringAlignment.Center;
                //найти название
                if (Schemes_Editor.mainList.Find(x => x.id == globalId) != null)
                {
                    string roomName = Schemes_Editor.mainList.Find(x => x.id == globalId).name;
                    g.DrawString(roomName, new Font("Arial", 10), Brushes.DarkRed, new RectangleF(locations[localSheetIndex].X, locations[localSheetIndex].Y - 30, scales[localSheetIndex].X, 30), f);

                }

            }
        }

        public override void drawStr(Graphics g)
        {
            int localSheetIndex = 3;

            g.DrawLines(new Pen(Brushes.Black, 3), new System.Drawing.Point[] { locations[localSheetIndex],
                new System.Drawing.Point(locations[localSheetIndex].X + scales[localSheetIndex].X, locations[localSheetIndex].Y),
                new System.Drawing.Point(locations[localSheetIndex].X + scales[localSheetIndex].X, locations[localSheetIndex].Y + scales[localSheetIndex].Y),
                new System.Drawing.Point(locations[localSheetIndex].X, locations[localSheetIndex].Y + scales[localSheetIndex].Y),
                locations[localSheetIndex] });


            StringFormat f = new StringFormat();
            f.Alignment = StringAlignment.Center;
            f.LineAlignment = StringAlignment.Center;
            //найти название
            if (Schemes_Editor.mainList.Find(x => x.id == globalId) != null)
            {
                g.DrawString(strLabel, new Font("Arial", 10), Brushes.DarkRed, new RectangleF(locations[localSheetIndex].X, locations[localSheetIndex].Y, scales[localSheetIndex].X, scales[localSheetIndex].Y));

            }

            //if (vinoska != null)
            //{
            //    g.DrawLines(Pens.Blue, new Point[] { vinoska.startPoint, vinoska.vertex1, vinoska.vertex2 });
            //    g.DrawString(vinoska.text, new Font("Arial", 14), Brushes.DarkGreen, vinoska.vertex1.X, vinoska.vertex1.Y - 30);
            //}
        }

        public override void offset(System.Drawing.Point offset, int scheme)
        {

        }

        public override void drawPlaceExp(ref string result)
        {
        }


        public override void drawConExp(ref string result)
        {
            if (seized.Count == 0)
                foreach (var i in Schemes_Editor.mainList.Find(x => x.id == globalId).compatibilities)
                    seized.Add(0);

            int localSheetIndex = 1;
            List<Equipment.VectorPic> list = new List<Equipment.VectorPic>();
            if (Schemes_Editor.mainList.Find(x => x.id == globalId).inPlacementScheme != null)
                list.Add(Schemes_Editor.mainList.Find(x => x.id == globalId).inPlacementScheme.copy());
            else
                list.Add(null);

            if (Schemes_Editor.mainList.Find(x => x.id == globalId).inConnectionScheme != null)
                list.Add(Schemes_Editor.mainList.Find(x => x.id == globalId).inConnectionScheme.copy());
            else
                list.Add(null);

            if (Schemes_Editor.mainList.Find(x => x.id == globalId).inBox != null)
                list.Add(Schemes_Editor.mainList.Find(x => x.id == globalId).inBox.copy());
            else
                list.Add(null);

            if (Schemes_Editor.mainList.Find(x => x.id == globalId).inStructural != null)
                list.Add(Schemes_Editor.mainList.Find(x => x.id == globalId).inStructural.copy());
            else
                list.Add(null);

            if (list[localSheetIndex] != null)
            {
                Equipment.Point WidthHeight = list[localSheetIndex].GetProp();
                float Xprop = WidthHeight.X / (scales[localSheetIndex].X * 1.0f),
                    Yprop = WidthHeight.Y / (scales[localSheetIndex].Y * 1.0f);
                list[localSheetIndex].divide(Xprop, Yprop);

                //foreach (var j in list[localSheetIndex].circles)
                //    g.DrawEllipse(Pens.Black, locations[localSheetIndex].X + (float)j.center.X - (float)j.radiusX, locations[localSheetIndex].Y + (float)j.center.Y - (float)j.radiusY, (float)j.radiusX * 2, (float)j.radiusY * 2);

                foreach (var j in list[localSheetIndex].polyLines)
                {
                    for (int k = 0; k < j.Count - 1; k++)
                    {
                        //        result += $"pline {j[k].X + locations[localSheetIndex].X+1500},{-j[k].Y - locations[localSheetIndex].Y} {j[k + 1].X + locations[localSheetIndex].X + 1500},{j[k + 1].Y + locations[localSheetIndex].Y} \r\n";
                        //   g.DrawLine(Pens.Black, j[k].X + locations[localSheetIndex].X, j[k].Y + locations[localSheetIndex].Y, j[k + 1].X + locations[localSheetIndex].X, j[k + 1].Y + locations[localSheetIndex].Y);
                    }
                }

                StringFormat f = new StringFormat();
                f.Alignment = StringAlignment.Center;
                //найти название
                if (Schemes_Editor.mainList.Find(x => x.id == globalId) != null)
                {
                    string roomName = Schemes_Editor.mainList.Find(x => x.id == globalId).name;
                    //         result += $"text {locations[localSheetIndex].X},{-locations[localSheetIndex].Y + 30} 0 {roomName}\r\n";
                    //    g.DrawString(roomName, new Font("Arial", 10), Brushes.DarkRed, new RectangleF(locations[localSheetIndex].X, locations[localSheetIndex].Y - 30, scales[localSheetIndex].X, 30), f);

                }

            }
        }

        public override void drawBoxExp(ref string result)
        {
            int localSheetIndex = 2;
            List<Equipment.VectorPic> list = new List<Equipment.VectorPic>();
            if (Schemes_Editor.mainList.Find(x => x.id == globalId).inPlacementScheme != null)
                list.Add(Schemes_Editor.mainList.Find(x => x.id == globalId).inPlacementScheme.copy());
            else
                list.Add(null);

            if (Schemes_Editor.mainList.Find(x => x.id == globalId).inConnectionScheme != null)
                list.Add(Schemes_Editor.mainList.Find(x => x.id == globalId).inConnectionScheme.copy());
            else
                list.Add(null);

            if (Schemes_Editor.mainList.Find(x => x.id == globalId).inBox != null)
                list.Add(Schemes_Editor.mainList.Find(x => x.id == globalId).inBox.copy());
            else
                list.Add(null);

            if (Schemes_Editor.mainList.Find(x => x.id == globalId).inStructural != null)
                list.Add(Schemes_Editor.mainList.Find(x => x.id == globalId).inStructural.copy());
            else
                list.Add(null);

            if (list[localSheetIndex] != null)
            {
                Equipment.Point WidthHeight = list[localSheetIndex].GetProp();
                float Xprop = WidthHeight.X / (scales[localSheetIndex].X * 1.0f),
                    Yprop = WidthHeight.Y / (scales[localSheetIndex].Y * 1.0f);
                list[localSheetIndex].divide(Xprop, Yprop);

                //foreach (var j in list[localSheetIndex].circles)
                //    g.DrawEllipse(Pens.Black, locations[localSheetIndex].X + (float)j.center.X - (float)j.radiusX, locations[localSheetIndex].Y + (float)j.center.Y - (float)j.radiusY, (float)j.radiusX * 2, (float)j.radiusY * 2);

                foreach (var j in list[localSheetIndex].polyLines)
                {
                    for (int k = 0; k < j.Count - 1; k++)
                    {
                        result += AutocadExport.drawLine(locations[localSheetIndex].X+j[k].X, locations[localSheetIndex].Y + j[k].Y, locations[localSheetIndex].X + j[k+1].X, locations[localSheetIndex].Y + j[k+1].Y);
                    //    result += $"pline {j[k].X + locations[localSheetIndex].X},{-j[k].Y - locations[localSheetIndex].Y} {j[k + 1].X + locations[localSheetIndex].X},{-j[k + 1].Y - locations[localSheetIndex].Y} \r\n";
                        // g.DrawLine(Pens.Black, j[k].X + locations[localSheetIndex].X, j[k].Y + locations[localSheetIndex].Y, j[k + 1].X + locations[localSheetIndex].X, j[k + 1].Y + locations[localSheetIndex].Y);
                    }
                }
            }
            if (vinoska != null)
            {
                //g.DrawLines(Pens.Blue, new System.Drawing.Point[] { vinoska.startPoint, vinoska.vertex1, vinoska.vertex2 });
                //g.DrawString(vinoska.text, new Font("Arial", 14), Brushes.DarkGreen, vinoska.vertex1.X, vinoska.vertex1.Y - 30);
            }
        }

        public override void drawStrExp(ref string result)
        {

        }
    }
}
