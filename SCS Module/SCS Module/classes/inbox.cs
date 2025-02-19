﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SCS_Module
{
    public class inboxes : drawer
    {
        int offsetConnection = 3000;




        public List<int> seized = new List<int>();
        public bool isShowingOnConnectionScheme = true;
    //    public List<UsedInterface> listINterfaces = new List<UsedInterface>();
        public bool inbox = false;
        public int numberOfUnits = -1;
        public override void move(Point offset, int scheme)
        {
            locations[scheme] = offset;
            if (vinoska != null)
            {
                rebuildVinosku(scheme);
                //vinoska.startPoint = new Point(locations[scheme].X + scales[scheme].X / 2, locations[scheme].Y + scales[scheme].Y / 2);

                //vinoska.vertex1 = new Point(offset.X + locations[scheme].X - vinoska.vertex1.X, offset.Y + locations[scheme].Y - vinoska.vertex1.Y);
                //vinoska.vertex2 = new Point(offset.X + locations[scheme].X - vinoska.vertex2.X, offset.Y + locations[scheme].Y - vinoska.vertex2.Y);
            }


        }
        public override bool inside(Point a, int scheme)
        {
            bool result = false;
            if (a.X > locations[scheme].X && a.X < locations[scheme].X + scales[scheme].X)
                if (a.Y > locations[scheme].Y && a.Y < locations[scheme].Y + scales[scheme].Y)
                    result = true;
            return result;
        }
        public override void drawBox(Graphics g)
        {
            if (labels == null)
            {
                string name = Schemes_Editor.mainList.Find(x => x.id == globalId).name;
                labels = new string[] { name, name, name, name };
            }
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
            if (vinoska != null && vinoska[localSheetIndex] != null)
            {
                if (localSheetIndex == 2 && inbox)
                {
                    int index = 0;
                    //найти коробку в которой он
                    foreach(boxes i in Schemes_Editor.mainWorkList.Where(x=>x is boxes))
                    {
                        int founded = i.equipInside.FindIndex(x => x.localID == localID);
                        if (founded != -1)
                        {
                            index = i.positions[founded] + 1;
                            break;
                        }
                    }
                    g.DrawLines(Pens.Blue, new Point[] { vinoska[localSheetIndex].startPoint, vinoska[localSheetIndex].vertex1, vinoska[localSheetIndex].vertex2 });
                    if (vinoska[localSheetIndex].vertex1.X < vinoska[localSheetIndex].vertex2.X)
                        g.DrawString(index.ToString(), new Font("Arial", 14), Brushes.DarkGreen, vinoska[localSheetIndex].vertex1.X, vinoska[localSheetIndex].vertex1.Y - 30);
                    else
                        g.DrawString(index.ToString(), new Font("Arial", 14), Brushes.DarkGreen, vinoska[localSheetIndex].vertex2.X, vinoska[localSheetIndex].vertex1.Y - 30);
                }
                else
                {
                    g.DrawLines(Pens.Blue, new Point[] { vinoska[localSheetIndex].startPoint, vinoska[localSheetIndex].vertex1, vinoska[localSheetIndex].vertex2 });
                    if (vinoska[localSheetIndex].vertex1.X < vinoska[localSheetIndex].vertex2.X)
                        g.DrawString(labels[localSheetIndex], new Font("Arial", 14), Brushes.DarkGreen, vinoska[localSheetIndex].vertex1.X, vinoska[localSheetIndex].vertex1.Y - 30);
                    else
                        g.DrawString(labels[localSheetIndex], new Font("Arial", 14), Brushes.DarkGreen, vinoska[localSheetIndex].vertex2.X, vinoska[localSheetIndex].vertex1.Y - 30);
                }
            }
        }

        public override void createVinosku(int indexx)
        {
            if (inbox)
            {
                //поиск целевого шкафа
                int index = ((boxes)Schemes_Editor.mainWorkList.Find(x => x is boxes && ((boxes)x).equipInside.Exists(y => y.localID == localID))).equipInside.IndexOf(this) + 1;
                vinoska[indexx] = new Vinoska(labels[indexx], new Point(locations[2].X + scales[2].X / 2, locations[2].Y + scales[2].Y / 2), locations[Schemes_Editor.sheetIndex], new Point(locations[Schemes_Editor.sheetIndex].X + 30, locations[Schemes_Editor.sheetIndex].Y));
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
                    // string roomName = Schemes_Editor.mainList.Find(x => x.id == globalId).name;
                    g.DrawString(labels[localSheetIndex], new Font("Arial", 10), Brushes.DarkRed, new RectangleF(locations[localSheetIndex].X, locations[localSheetIndex].Y - 30, scales[localSheetIndex].X, 30), f);

                }

            }
        }

        public override void drawStr(Graphics g)
        {
            int localSheetIndex = 3;
            if (!inbox) return;
            g.DrawLines(new Pen(Brushes.Black, 3), new Point[] { locations[localSheetIndex],
                new Point(locations[localSheetIndex].X + scales[localSheetIndex].X, locations[localSheetIndex].Y),
                new Point(locations[localSheetIndex].X + scales[localSheetIndex].X, locations[localSheetIndex].Y + scales[localSheetIndex].Y),
                new Point(locations[localSheetIndex].X, locations[localSheetIndex].Y + scales[localSheetIndex].Y),
                locations[localSheetIndex] });


            StringFormat f = new StringFormat();
            f.Alignment = StringAlignment.Center;
            //найти название
            if (Schemes_Editor.mainList.Find(x => x.id == globalId) != null)
            {
                g.DrawString(labels[localSheetIndex], new Font("Arial", 7), Brushes.DarkRed, new RectangleF(locations[localSheetIndex].X, locations[localSheetIndex].Y, scales[localSheetIndex].X, scales[localSheetIndex].Y), f);

            }

            //if (vinoska != null)
            //{
            //    g.DrawLines(Pens.Blue, new Point[] { vinoska.startPoint, vinoska.vertex1, vinoska.vertex2 });
            //    g.DrawString(vinoska.text, new Font("Arial", 14), Brushes.DarkGreen, vinoska.vertex1.X, vinoska.vertex1.Y - 30);
            //}
        }

        public override void offset(Point offset, int scheme)
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
                        result += AutocadExport.drawLine(offsetConnection+j[k].X + locations[localSheetIndex].X, j[k].Y + locations[localSheetIndex].Y, offsetConnection + j[k + 1].X + locations[localSheetIndex].X, j[k + 1].Y + locations[localSheetIndex].Y);
                       // g.DrawLine(Pens.Black, j[k].X + locations[localSheetIndex].X, j[k].Y + locations[localSheetIndex].Y, j[k + 1].X + locations[localSheetIndex].X, j[k + 1].Y + locations[localSheetIndex].Y);
                    }
                }

                StringFormat f = new StringFormat();
                f.Alignment = StringAlignment.Center;
                //найти название
                if (Schemes_Editor.mainList.Find(x => x.id == globalId) != null)
                {
                    // string roomName = Schemes_Editor.mainList.Find(x => x.id == globalId).name;
                    result += AutocadExport.drawText(new RectangleF(offsetConnection + locations[localSheetIndex].X, locations[localSheetIndex].Y - 30, scales[localSheetIndex].X, 30), labels[localSheetIndex]);
                    //g.DrawString(labels[localSheetIndex], new Font("Arial", 10), Brushes.DarkRed, new RectangleF(locations[localSheetIndex].X, locations[localSheetIndex].Y - 30, scales[localSheetIndex].X, 30), f);

                }

            }
        }

        public override void drawBoxExp(ref string result)
        {
            if (labels == null)
            {
                string name = Schemes_Editor.mainList.Find(x => x.id == globalId).name;
                labels = new string[] { name, name, name, name };
            }
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
                        result += AutocadExport.drawLine(offsetConnection*2+j[k].X + locations[localSheetIndex].X, j[k].Y + locations[localSheetIndex].Y, offsetConnection * 3 + j[k + 1].X + locations[localSheetIndex].X, j[k + 1].Y + locations[localSheetIndex].Y);
                        //g.DrawLine(Pens.Black, j[k].X + locations[localSheetIndex].X, j[k].Y + locations[localSheetIndex].Y, j[k + 1].X + locations[localSheetIndex].X, j[k + 1].Y + locations[localSheetIndex].Y);
                    }
                }
            }
            if (vinoska != null && vinoska[localSheetIndex] != null)
            {
                if (localSheetIndex == 2 && inbox)
                {
                    int index = 0;
                    //найти коробку в которой он
                    foreach (boxes i in Schemes_Editor.mainWorkList.Where(x => x is boxes))
                    {
                        int founded = i.equipInside.FindIndex(x => x.localID == localID);
                        if (founded != -1)
                        {
                            index = i.positions[founded] + 1;
                            break;
                        }
                    }
                    result += AutocadExport.drawLine(new Point(offsetConnection * 2 + vinoska[localSheetIndex].startPoint.X, vinoska[localSheetIndex].startPoint.Y), new Point(offsetConnection * 3+ vinoska[localSheetIndex].vertex1.X, vinoska[localSheetIndex].vertex1.Y));
                    //   g.DrawLines(Pens.Blue, new Point[] { vinoska[localSheetIndex].startPoint, vinoska[localSheetIndex].vertex1, vinoska[localSheetIndex].vertex2 });
                    if (vinoska[localSheetIndex].vertex1.X < vinoska[localSheetIndex].vertex2.X)
                        result += AutocadExport.drawText(offsetConnection * 2+vinoska[localSheetIndex].vertex1.X, vinoska[localSheetIndex].vertex1.Y - 30, 50,30, index.ToString());
                     //   g.DrawString(index.ToString(), new Font("Arial", 14), Brushes.DarkGreen, vinoska[localSheetIndex].vertex1.X, vinoska[localSheetIndex].vertex1.Y - 30);
                    else
                        result += AutocadExport.drawText(offsetConnection * 2+vinoska[localSheetIndex].vertex2.X, vinoska[localSheetIndex].vertex1.Y - 30, 50, 30, index.ToString());
                 //   g.DrawString(index.ToString(), new Font("Arial", 14), Brushes.DarkGreen, vinoska[localSheetIndex].vertex2.X, vinoska[localSheetIndex].vertex1.Y - 30);
                }
                else
                {
                    result += AutocadExport.drawLine(new Point(offsetConnection * 2+ vinoska[localSheetIndex].startPoint.X, vinoska[localSheetIndex].startPoint.Y), new Point(offsetConnection * 3 + vinoska[localSheetIndex].vertex1.X, vinoska[localSheetIndex].vertex1.Y));
                 //   g.DrawLines(Pens.Blue, new Point[] { vinoska[localSheetIndex].startPoint, vinoska[localSheetIndex].vertex1, vinoska[localSheetIndex].vertex2 });
                    if (vinoska[localSheetIndex].vertex1.X < vinoska[localSheetIndex].vertex2.X)
                        result += AutocadExport.drawText(offsetConnection * 2 + vinoska[localSheetIndex].vertex1.X, vinoska[localSheetIndex].vertex1.Y - 30, 50, 30, labels[localSheetIndex]);
                   // g.DrawString(labels[localSheetIndex], new Font("Arial", 14), Brushes.DarkGreen, vinoska[localSheetIndex].vertex1.X, vinoska[localSheetIndex].vertex1.Y - 30);
                    else
                                            result += AutocadExport.drawText(offsetConnection * 2 + vinoska[localSheetIndex].vertex2.X, vinoska[localSheetIndex].vertex1.Y - 30, 50, 30, labels[localSheetIndex]);
                   // g.DrawString(labels[localSheetIndex], new Font("Arial", 14), Brushes.DarkGreen, vinoska[localSheetIndex].vertex2.X, vinoska[localSheetIndex].vertex1.Y - 30);
                }
            }
        }

        public override void drawStrExp(ref string result)
        {
            int localSheetIndex = 3;
            if (!inbox) return;
            result += AutocadExport.drawrect(new  Rectangle(offsetConnection * 3 + locations[localSheetIndex].X, locations[localSheetIndex].Y, scales[localSheetIndex].X, scales[localSheetIndex].Y));
            //g.DrawLines(new Pen(Brushes.Black, 3), new Point[] { locations[localSheetIndex],
            //    new Point(locations[localSheetIndex].X + scales[localSheetIndex].X, locations[localSheetIndex].Y),
            //    new Point(locations[localSheetIndex].X + scales[localSheetIndex].X, locations[localSheetIndex].Y + scales[localSheetIndex].Y),
            //    new Point(locations[localSheetIndex].X, locations[localSheetIndex].Y + scales[localSheetIndex].Y),
            //    locations[localSheetIndex] });


            StringFormat f = new StringFormat();
            f.Alignment = StringAlignment.Center;
            f.LineAlignment = StringAlignment.Center;
            //найти название
            if (Schemes_Editor.mainList.Find(x => x.id == globalId) != null)
            {
                result += AutocadExport.drawText(new RectangleF(offsetConnection * 3 + locations[localSheetIndex].X, locations[localSheetIndex].Y, scales[localSheetIndex].X, scales[localSheetIndex].Y), labels[localSheetIndex]);
              //  g.DrawString(labels[localSheetIndex], new Font("Arial", 7), Brushes.DarkRed, new RectangleF(locations[localSheetIndex].X, locations[localSheetIndex].Y, scales[localSheetIndex].X, scales[localSheetIndex].Y));

            }

            //if (vinoska != null)
            //{
            //    g.DrawLines(Pens.Blue, new Point[] { vinoska.startPoint, vinoska.vertex1, vinoska.vertex2 });
            //    g.DrawString(vinoska.text, new Font("Arial", 14), Brushes.DarkGreen, vinoska.vertex1.X, vinoska.vertex1.Y - 30);
            //}
        }

        public override void rebuildVinosku(int index)
        {
            if (vinoska != null && vinoska[index]!=null)
            {
                Point a = vinoska[index].startPoint;
                Point b = vinoska[index].vertex1;
                Point c = vinoska[index].vertex2;
                Point newA = new Point(locations[2].X + scales[2].X / 2, locations[2].Y + scales[2].Y / 2);
                Point newB = new Point(newA.X+b.X-a.X, newA.Y+b.Y-a.Y);
                Point newc = new Point(newB.X + c.X - b.X, newB.Y + c.Y - b.Y);
                vinoska[index] = new Vinoska(vinoska[index].text, newA,newB , newc);
            }
        }
    }
}
